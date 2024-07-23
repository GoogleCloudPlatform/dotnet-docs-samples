/*
 * Copyright 2022 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Cloud.Video.Stitcher.V1;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xunit;

[CollectionDefinition(nameof(StitcherFixture))]
public class StitcherFixture : IDisposable, IAsyncLifetime, ICollectionFixture<StitcherFixture>
{
    public string ProjectId { get; }
    public string LocationId { get; } = "us-central1";
    public string SlateIdPrefix { get; } = "slate";
    public string AkamaiCdnKeyIdPrefix { get; } = "akamai-cdn";
    public string CloudCdnKeyIdPrefix { get; } = "cloud-cdn";
    public string MediaCdnKeyIdPrefix { get; } = "media-cdn";
    public string LiveConfigIdPrefix { get; } = "live-config";
    public string VodConfigIdPrefix { get; } = "vod-config";

    public List<string> SlateIds { get; } = new List<string>();
    public List<string> CdnKeyIds { get; } = new List<string>();
    public List<string> LiveConfigIds { get; } = new List<string>();
    public List<string> VodConfigIds { get; } = new List<string>();

    public Slate TestSlate { get; set; }
    public string TestSlateId { get; set; }

    public Slate TestSlateForLiveConfig { get; set; }
    public string TestSlateForLiveConfigId { get; set; }
    public string TestSlateUri { get; } = "https://storage.googleapis.com/cloud-samples-data/media/ForBiggerEscapes.mp4";
    public string UpdateSlateUri { get; } = "https://storage.googleapis.com/cloud-samples-data/media/ForBiggerJoyrides.mp4";
    public string VodSourceUri { get; } = "https://storage.googleapis.com/cloud-samples-data/media/hls-vod/manifest.m3u8";
    public string UpdateVodSourceUri { get; } = "https://storage.googleapis.com/cloud-samples-data/media/hls-vod/manifest.mpd";
    public string VodAdTagUri { get; } = "https://pubads.g.doubleclick.net/gampad/ads?iu=/21775744923/external/vmap_ad_samples&sz=640x480&cust_params=sample_ar%3Dpreonly&ciu_szs=300x250%2C728x90&gdfp_req=1&ad_rule=1&output=vmap&unviewed_position_start=1&env=vp&impl=s&correlator=";
    public string LiveSourceUri { get; } = "https://storage.googleapis.com/cloud-samples-data/media/hls-live/manifest.m3u8";
    public string LiveAdTagUri { get; } = "https://pubads.g.doubleclick.net/gampad/ads?iu=/21775744923/external/single_ad_samples&sz=640x480&cust_params=sample_ct%3Dlinear&ciu_szs=300x250%2C728x90&gdfp_req=1&output=vast&unviewed_position_start=1&env=vp&impl=s&correlator=";

    public string Hostname { get; } = "cdn.example.com";
    public string UpdatedAkamaiHostname { get; } = "updated.akamai.cdn.example.com";
    public string UpdatedCloudCdnHostname { get; } = "updated.cloud.cdn.example.com";
    public string UpdatedMediaCdnHostname { get; } = "updated.media.cdn.example.com";

    public CdnKey TestCloudCdnKey { get; set; }
    public string TestCloudCdnKeyId { get; set; }

    public CdnKey TestAkamaiCdnKey { get; set; }
    public string TestAkamaiCdnKeyId { get; set; }

    public string KeyName { get; } = "my-key";
    public string CloudCdnPrivateKey { get; } = "VGhpcyBpcyBhIHRlc3Qgc3RyaW5nLg==";
    public string MediaCdnPrivateKey { get; } = "MTIzNDU2Nzg5MDEyMzQ1Njc4Nzg5MDEyMzQ1Njc4OTAxMjM0NTY3ODkwMTIzNAAA";
    public string AkamaiTokenKey { get; } = "VGhpcyBpcyBhIHRlc3Qgc3RyaW5nLg==";
    public string UpdatedAkamaiTokenKey { get; } = "VGhpcyBpcyBhbiB1cGRhdGVkIHRlc3Qgc3RyaW5nLg==";

    public LiveConfig TestLiveConfig { get; set; }
    public string TestLiveConfigId { get; set; }
    public VodConfig TestVodConfig { get; set; }
    public string TestVodConfigId { get; set; }

    private readonly CreateSlateSample _createSlateSample = new CreateSlateSample();
    private readonly ListSlatesSample _listSlatesSample = new ListSlatesSample();
    private readonly DeleteSlateSample _deleteSlateSample = new DeleteSlateSample();
    private readonly CreateCdnKeySample _createCdnKeySample = new CreateCdnKeySample();
    private readonly CreateCdnKeyAkamaiSample _createCdnKeyAkamaiSample = new CreateCdnKeyAkamaiSample();

    private readonly ListCdnKeysSample _listCdnKeysSample = new ListCdnKeysSample();
    private readonly DeleteCdnKeySample _deleteCdnKeySample = new DeleteCdnKeySample();

    private readonly CreateLiveConfigSample _createLiveConfigSample = new CreateLiveConfigSample();
    private readonly ListLiveConfigsSample _listLiveConfigsSample = new ListLiveConfigsSample();
    private readonly DeleteLiveConfigSample _deleteLiveConfigSample = new DeleteLiveConfigSample();

    private readonly CreateVodConfigSample _createVodConfigSample = new CreateVodConfigSample();
    private readonly ListVodConfigsSample _listVodConfigsSample = new ListVodConfigsSample();
    private readonly DeleteVodConfigSample _deleteVodConfigSample = new DeleteVodConfigSample();

    private HttpClient httpClient;

    public StitcherFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }
    }
    public async Task InitializeAsync()
    {
        await CleanOutdatedResources();

        TestSlateId = $"{SlateIdPrefix}-{RandomId()}-{TimestampId()}";
        SlateIds.Add(TestSlateId);
        TestSlate = await _createSlateSample.CreateSlateAsync(ProjectId, LocationId, TestSlateId, TestSlateUri);

        TestSlateForLiveConfigId = $"{SlateIdPrefix}-{RandomId()}-{TimestampId()}";
        SlateIds.Add(TestSlateForLiveConfigId);
        TestSlateForLiveConfig = await _createSlateSample.CreateSlateAsync(ProjectId, LocationId, TestSlateForLiveConfigId, TestSlateUri);

        TestCloudCdnKeyId = $"{CloudCdnKeyIdPrefix}-{RandomId()}-{TimestampId()}";
        CdnKeyIds.Add(TestCloudCdnKeyId);
        TestCloudCdnKey = await _createCdnKeySample.CreateCdnKeyAsync(ProjectId, LocationId, TestCloudCdnKeyId, Hostname, KeyName, CloudCdnPrivateKey, false);

        TestAkamaiCdnKeyId = $"{AkamaiCdnKeyIdPrefix}-{RandomId()}-{TimestampId()}";
        CdnKeyIds.Add(TestAkamaiCdnKeyId);
        TestAkamaiCdnKey = await _createCdnKeyAkamaiSample.CreateCdnKeyAkamaiAsync(ProjectId, LocationId, TestAkamaiCdnKeyId, Hostname, AkamaiTokenKey);

        TestLiveConfigId = $"{LiveConfigIdPrefix}-{RandomId()}-{TimestampId()}";
        LiveConfigIds.Add(TestLiveConfigId);
        TestLiveConfig = await _createLiveConfigSample.CreateLiveConfigAsync(ProjectId, LocationId, TestLiveConfigId, LiveSourceUri, LiveAdTagUri, TestSlateForLiveConfigId);

        TestVodConfigId = $"{VodConfigIdPrefix}-{RandomId()}-{TimestampId()}";
        VodConfigIds.Add(TestVodConfigId);
        TestVodConfig = await _createVodConfigSample.CreateVodConfigAsync(ProjectId, LocationId, TestVodConfigId, VodSourceUri, VodAdTagUri);

        httpClient = new HttpClient();
    }

    public async Task CleanOutdatedResources()
    {
        int TWO_HOURS_IN_SECS = 7200;
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // Slates don't include creation time information, so encode it
        // in the slate name. Slates have a low quota limit, so we need to
        // remove outdated ones before the test begins (and creates more).
        var slates = _listSlatesSample.ListSlates(ProjectId, LocationId);
        foreach (Slate slate in slates)
        {
            string id = slate.SlateName.SlateId;
            string[] subs = id.Split('-');
            if (subs.Length > 0)
            {
                string temp = subs[(subs.Length - 1)];
                bool success = long.TryParse(temp, out long creation);
                if (success)
                {
                    if ((now - creation) >= TWO_HOURS_IN_SECS)
                    {
                        await DeleteSlate(id);
                    }
                }
            }
        }
        // CDN keys don't include creation time information, so encode it
        // in the key name. CDN keys have a low quota limit, so we need to
        // remove outdated ones before the test begins (and creates more).
        var cdnKeys = _listCdnKeysSample.ListCdnKeys(ProjectId, LocationId);
        foreach (CdnKey cdnKey in cdnKeys)
        {
            string id = cdnKey.CdnKeyName.CdnKeyId;
            string[] subs = id.Split('-');
            if (subs.Length > 0)
            {
                string temp = subs[(subs.Length - 1)];
                bool success = long.TryParse(temp, out long creation);
                if (success)
                {
                    if ((now - creation) >= TWO_HOURS_IN_SECS)
                    {
                        await DeleteCdnKey(id);
                    }
                }
            }
        }
        // Live configs don't include creation time information, so encode it
        // in the config name. Live configs have a low quota limit, so we need to
        // remove outdated ones before the test begins (and creates more).
        var liveConfigs = _listLiveConfigsSample.ListLiveConfigs(ProjectId, LocationId);
        foreach (LiveConfig liveConfig in liveConfigs)
        {
            string id = liveConfig.LiveConfigName.LiveConfigId;
            string[] subs = id.Split('-');
            if (subs.Length > 0)
            {
                string temp = subs[(subs.Length - 1)];
                bool success = long.TryParse(temp, out long creation);
                if (success)
                {
                    if ((now - creation) >= TWO_HOURS_IN_SECS)
                    {
                        await DeleteLiveConfig(id);
                    }
                }
            }
        }
        // VOD configs don't include creation time information, so encode it
        // in the config name. VOD configs have a low quota limit, so we need to
        // remove outdated ones before the test begins (and creates more).
        var vodConfigs = _listVodConfigsSample.ListVodConfigs(ProjectId, LocationId);
        foreach (VodConfig vodConfig in vodConfigs)
        {
            string id = vodConfig.VodConfigName.VodConfigId;
            string[] subs = id.Split('-');
            if (subs.Length > 0)
            {
                string temp = subs[(subs.Length - 1)];
                bool success = long.TryParse(temp, out long creation);
                if (success)
                {
                    if ((now - creation) >= TWO_HOURS_IN_SECS)
                    {
                        await DeleteVodConfig(id);
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }

    public async Task DisposeAsync()
    {
        foreach (string id in SlateIds)
        {
            await DeleteSlate(id);
        }

        foreach (string id in CdnKeyIds)
        {
            await DeleteCdnKey(id);
        }

        foreach (string id in LiveConfigIds)
        {
            await DeleteLiveConfig(id);
        }

        foreach (string id in VodConfigIds)
        {
            await DeleteVodConfig(id);
        }
    }

    public async Task DeleteSlate(string id)
    {
        try
        {
            await _deleteSlateSample.DeleteSlateAsync(ProjectId, LocationId, id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Delete failed for slate: " + id + " with error: " + e.ToString());
        }
    }

    public async Task DeleteCdnKey(string id)
    {
        try
        {
            await _deleteCdnKeySample.DeleteCdnKeyAsync(ProjectId, LocationId, id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Delete failed for CDN key: " + id + " with error: " + e.ToString());
        }
    }

    public async Task DeleteLiveConfig(string id)
    {
        try
        {
            await _deleteLiveConfigSample.DeleteLiveConfigAsync(ProjectId, LocationId, id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Delete failed for live config: " + id + " with error: " + e.ToString());
        }
    }

    public async Task DeleteVodConfig(string id)
    {
        try
        {
            await _deleteVodConfigSample.DeleteVodConfigAsync(ProjectId, LocationId, id);
        }
        catch (Exception e)
        {
            Console.WriteLine("Delete failed for VOD config: " + id + " with error: " + e.ToString());
        }
    }

    public string TimestampId()
    {
        return $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    }

    public string RandomId()
    {
        return $"{System.Guid.NewGuid()}";
    }

    public async Task<String> GetHttpResponse(string url)
    {
        using (var response = await httpClient.GetAsync(url))
        {
            return await response.Content.ReadAsStringAsync();
        }
    }

    public async Task GetManifestAndRendition(string playUri)
    {
        // To get ad tag details, you need to make a request to the main manifest and
        // a rendition first. This supplies media player information to the API.
        //
        // Curl the playUri first. The last line of the response will contain a
        // renditions location. Curl the live session name with the rendition
        // location appended.
        string renditions = await GetHttpResponse(playUri);
        Match m = Regex.Match(renditions, "renditions/.*", RegexOptions.IgnoreCase);

        // playUri will be in the following format:
        // https://videostitcher.googleapis.com/v1/projects/{project}/locations/{location}/liveSessions/{session-id}/manifest.m3u8?signature=...
        // Replace manifest.m3u8?signature=... with the renditions location.
        string tmp = playUri.Substring(0, playUri.LastIndexOf("/"));
        string stitchedUrl = $"{tmp}/{m.Value}";
        await GetHttpResponse(stitchedUrl); // Curl the live session name with rendition
    }
}