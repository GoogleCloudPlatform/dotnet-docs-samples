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

using Xunit;

[CollectionDefinition(nameof(StitcherFixture))]
public class StitcherFixture : IDisposable, ICollectionFixture<StitcherFixture>
{
    public string ProjectId { get; }
    public string LocationId { get; } = "us-central1";
    public string SlateIdPrefix { get; } = "test-slate";
    public string AkamaiCdnKeyIdPrefix { get; } = "test-akamai-cdn-key";
    public string CloudCdnKeyIdPrefix { get; } = "test-cloud-cdn-key";

    public List<string> SlateIds { get; } = new List<string>();
    public List<string> CdnKeyIds { get; } = new List<string>();

    public Slate TestSlate { get; set; }
    public string TestSlateId { get; set; }
    public string TestSlateUri { get; } = "https://storage.googleapis.com/cloud-samples-data/media/ForBiggerEscapes.mp4";
    public string UpdateSlateUri { get; } = "https://storage.googleapis.com/cloud-samples-data/media/ForBiggerJoyrides.mp4";

    public string Hostname { get; } = "cdn.example.com";
    public string UpdateHostname { get; } = "update.cdn.example.com";

    public CdnKey TestCloudCdnKey { get; set; }
    public string TestCloudCdnKeyId { get; set; }
    public string CloudCdnKeyName { get; } = "gcdn-key";
    public string CloudCdnTokenKey { get; } = "VGhpcyBpcyBhIHRlc3Qgc3RyaW5nLg==";
    public string UpdatedCloudCdnTokenKey = "VGhpcyBpcyBhbiB1cGRhdGVkIHRlc3Qgc3RyaW5nLg==";
    public string AkamaiTokenKey { get; } = "VGhpcyBpcyBhIHRlc3Qgc3RyaW5nLg==";

    private readonly CreateSlateSample _createSlateSample = new CreateSlateSample();
    private readonly DeleteSlateSample _deleteSlateSample = new DeleteSlateSample();
    private readonly CreateCdnKeySample _createCdnKeySample = new CreateCdnKeySample();
    private readonly DeleteCdnKeySample _deleteCdnKeySample = new DeleteCdnKeySample();

    public StitcherFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        TestSlateId = $"{SlateIdPrefix}-{RandomId()}";
        SlateIds.Add(TestSlateId);
        TestSlate = _createSlateSample.CreateSlate(ProjectId, LocationId, TestSlateId, TestSlateUri);

        TestCloudCdnKeyId = $"{CloudCdnKeyIdPrefix}-{RandomId()}";
        CdnKeyIds.Add(TestCloudCdnKeyId);
        TestCloudCdnKey = _createCdnKeySample.CreateCdnKey(ProjectId, LocationId, TestCloudCdnKeyId, Hostname, CloudCdnKeyName, CloudCdnTokenKey, "");
    }

    public void Dispose()
    {
        // Delete slates.
        foreach (string id in SlateIds)
        {
            try
            {
                _deleteSlateSample.DeleteSlate(ProjectId, LocationId, id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for slate: " + id + " with error: " + e.ToString());
            }
        }
        // Delete CDN keys.
        foreach (string id in CdnKeyIds)
        {
            try
            {
                _deleteCdnKeySample.DeleteCdnKey(ProjectId, LocationId, id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for CDN key: " + id + " with error: " + e.ToString());
            }
        }
    }

    public string RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }
}