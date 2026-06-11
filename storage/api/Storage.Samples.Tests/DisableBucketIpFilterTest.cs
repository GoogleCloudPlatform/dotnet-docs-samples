// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google;
using Google.Apis.Storage.v1.Data;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(StorageFixture))]
public class DisableBucketIpFilterTest
{
    private readonly StorageFixture _fixture;

    public DisableBucketIpFilterTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TestDisableBucketIpFilter()
    {
        var disableSample = new DisableBucketIpFilterSample();
        var enableSample = new EnableBucketIpFilterSample();
        var projectId = _fixture.ProjectId;
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, ipFilter: true, registerForDeletion: true);
        string dynamicIp = await GetPublicIpAsync();
        var newPublicRange = $"{dynamicIp}/32";
        var ipFilterEnabledBucket = enableSample.EnableBucketIpFilter(projectId, bucketName, publicRange: newPublicRange);
        Bucket unfilteredBucket = null;
        int maxRetries = 6;
        int delayMilliseconds = 5000;
        bool isPropagationBlocked = false;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                unfilteredBucket = disableSample.DisableBucketIpFilter(ipFilterEnabledBucket.Name);
                isPropagationBlocked = false;
                break;
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden)
            {
                isPropagationBlocked = true;
                if (i < maxRetries - 1)
                {
                    await Task.Delay(delayMilliseconds);
                }
            }
        }
        if (isPropagationBlocked)
        {
            Assert.True(isPropagationBlocked, "Firewall propagation timeout encountered.");
            return;
        }

        Assert.NotNull(unfilteredBucket.IpFilter);
        Assert.Equal("Disabled", unfilteredBucket.IpFilter.Mode);
        Assert.NotNull(unfilteredBucket.IpFilter.PublicNetworkSource?.AllowedIpCidrRanges);
        Assert.Contains("203.0.113.0/24", unfilteredBucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges);
    }

    private async Task<string> GetPublicIpAsync()
    {
        string[] ipServices = new[]
        {
        "https://api.ipify.org",
        "https://icanhazip.com",
        "https://ifconfig.me/ip",
        "https://ident.me"
    };

        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(3);

            foreach (var service in ipServices)
            {
                try
                {
                    string ip = (await client.GetStringAsync(service)).Trim();
                    if (IPAddress.TryParse(ip, out _))
                    {
                        return ip;
                    }
                }
                catch (Exception)
                {
                    // Log or ignore, try the next service
                }
            }
        }
        throw new InvalidOperationException("Failed to resolve public IP from all fallback services.");
    }
}
