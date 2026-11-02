// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google;
using Google.Cloud.Storage.Control.V2;
using Google.Cloud.Storage.V1;
using Google.LongRunning;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using Xunit;
using Bucket = Google.Apis.Storage.v1.Data.Bucket;

namespace Storage.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class StorageControlRapidCacheTest
{
    // Poll for up to 2 minutes for the cache to reach the "disabled" state.
    private const int DisablePollAttempts = 12;
    private static readonly TimeSpan s_disablePollInterval = TimeSpan.FromSeconds(10);

    // Retry bucket deletion for up to 2 minutes while the disabled cache is being removed.
    private static readonly TimeSpan s_bucketDeleteTimeout = TimeSpan.FromSeconds(120);
    private static readonly TimeSpan s_bucketDeleteInterval = TimeSpan.FromSeconds(15);

    private readonly StorageFixture _fixture;
    private readonly string _zone = Environment.GetEnvironmentVariable("RAPID_CACHE_ZONE") ?? "us-central1-a";

    public StorageControlRapidCacheTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    // Rapid Cache requires an allowlisted project and the bucket can't be deleted until the
    // disabled cache has been fully removed (which can take more than 30 minutes), so this
    // test is excluded from CI, following the Anywhere Cache samples which have no CI tests.
    [Fact(Skip = "b/482946642: Rapid Cache requires an allowlisted project; run manually.")]
    public void TestStorageControlRapidCacheLifecycle()
    {
        string bucketName = _fixture.GenerateBucketName();
        CreateRegionalHnsBucket(bucketName);
        try
        {
            RunRapidCacheLifecycle(bucketName);
        }
        finally
        {
            DeleteBucketBestEffort(bucketName);
        }
    }

    private void RunRapidCacheLifecycle(string bucketName)
    {
        // Create
        var createSample = new StorageControlCreateRapidCacheSample();
        RapidCache created = createSample.StorageControlCreateRapidCache(bucketName, _zone);
        Assert.Contains($"buckets/{bucketName}/rapidCaches/", created.Name);
        // The cache ID is assigned by the server; parse it from the created name.
        string rapidCacheId = RapidCacheName.Parse(created.Name).RapidCacheId;
        Assert.False(string.IsNullOrEmpty(rapidCacheId));

        var disableSample = new StorageControlDisableRapidCacheSample();
        try
        {
            // Get
            var getSample = new StorageControlGetRapidCacheSample();
            RapidCache fetched = getSample.StorageControlGetRapidCache(bucketName, rapidCacheId);
            Assert.Equal(created.Name, fetched.Name);
            Assert.Equal(_zone, fetched.Zone);

            // List
            var listSample = new StorageControlListRapidCachesSample();
            var caches = listSample.StorageControlListRapidCaches(bucketName).ToList();
            Assert.Contains(caches, cache => cache.Name == created.Name);

            // Update
            var updateSample = new StorageControlUpdateRapidCacheSample();
            RapidCache updated = updateSample.StorageControlUpdateRapidCache(bucketName, rapidCacheId, 172800);
            Assert.Equal(created.Name, updated.Name);
            Assert.Equal(172800L, updated.Ttl.Seconds);
        }
        catch
        {
            // Best-effort disable so the cache is cleaned up, without hiding the original failure.
            try
            {
                disableSample.StorageControlDisableRapidCache(bucketName, rapidCacheId);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not disable Rapid Cache {created.Name}: {e.Message}");
            }
            throw;
        }

        // Disable. This also cleans up the cache so that the bucket can be deleted once the
        // cache has been fully removed.
        Operation<RapidCache, DisableRapidCacheMetadata> disableOperation =
            disableSample.StorageControlDisableRapidCache(bucketName, rapidCacheId);
        Assert.False(string.IsNullOrEmpty(disableOperation.Name));

        // The cache moves to the "disabled" state shortly after the request.
        var getAfterDisableSample = new StorageControlGetRapidCacheSample();
        string state = null;
        for (int attempt = 0; attempt < DisablePollAttempts; attempt++)
        {
            state = getAfterDisableSample.StorageControlGetRapidCache(bucketName, rapidCacheId).State;
            if (state == "disabled")
            {
                break;
            }
            Thread.Sleep(s_disablePollInterval);
        }
        Assert.Equal("disabled", state);
    }

    // Rapid Cache Ultra requires a regional, hierarchical namespace enabled bucket in the
    // region of the cache zone.
    private void CreateRegionalHnsBucket(string bucketName)
    {
        string region = _zone.Substring(0, _zone.LastIndexOf('-'));
        _fixture.Client.CreateBucket(_fixture.ProjectId, new Bucket
        {
            Name = bucketName,
            Location = region,
            IamConfiguration = new Bucket.IamConfigurationData
            {
                UniformBucketLevelAccess = new Bucket.IamConfigurationData.UniformBucketLevelAccessData { Enabled = true }
            },
            HierarchicalNamespace = new Bucket.HierarchicalNamespaceData { Enabled = true }
        });
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }

    // A bucket can't be deleted (409 Conflict) until its disabled Rapid Cache has been fully
    // removed, which can take more than 30 minutes. Cleanup is best-effort: retry briefly,
    // then leave the bucket behind for the stale bucket cleanup.
    private void DeleteBucketBestEffort(string bucketName)
    {
        DateTime deadline = DateTime.UtcNow + s_bucketDeleteTimeout;
        while (true)
        {
            try
            {
                _fixture.Client.DeleteBucket(bucketName, new DeleteBucketOptions { DeleteObjects = true });
                return;
            }
            catch (GoogleApiException e) when (e.HttpStatusCode == HttpStatusCode.Conflict && DateTime.UtcNow < deadline)
            {
                Thread.Sleep(s_bucketDeleteInterval);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not delete bucket {bucketName}: {e.Message}");
                // Give the fixture one more best-effort attempt when it is disposed.
                _fixture.TempBucketNames.Add(bucketName);
                return;
            }
        }
    }
}
