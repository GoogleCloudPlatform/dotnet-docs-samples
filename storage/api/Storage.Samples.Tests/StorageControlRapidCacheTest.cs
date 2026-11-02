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
using Grpc.Core;
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
    private const string RapidCacheType = "rapid-cache-ultra";

    // Wait for the new bucket's metadata to propagate before creating the cache.
    private static readonly TimeSpan s_bucketPropagationDelay = TimeSpan.FromSeconds(20);

    // The disable operation only completes once the cache has been fully removed, which can
    // take more than an hour, so the test bounds the wait.
    private static readonly TimeSpan s_disableTimeout = TimeSpan.FromMinutes(10);

    // Short wait used for the best-effort disable on failure, so that cleanup never hangs.
    private static readonly TimeSpan s_cleanupDisableTimeout = TimeSpan.FromSeconds(1);

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
        // The Rapid Cache ID must be the same as the zone.
        string rapidCacheId = _zone;
        string rapidCacheName = RapidCacheName.FormatProjectBucketRapidCache("_", bucketName, rapidCacheId);

        // Create
        var createSample = new StorageControlCreateRapidCacheSample();
        RapidCache created = createSample.StorageControlCreateRapidCache(bucketName, _zone);
        Assert.Equal(rapidCacheName, created.Name);
        Assert.Equal(RapidCacheType, created.CacheType);

        var disableSample = new StorageControlDisableRapidCacheSample();
        var getSample = new StorageControlGetRapidCacheSample();
        try
        {
            // Get
            RapidCache fetched = getSample.StorageControlGetRapidCache(bucketName, rapidCacheId);
            Assert.Equal(rapidCacheName, fetched.Name);
            Assert.Equal(_zone, fetched.Zone);
            Assert.Equal("running", fetched.State);
            Assert.Equal(RapidCacheType, fetched.CacheType);

            // List
            var listSample = new StorageControlListRapidCachesSample();
            var caches = listSample.StorageControlListRapidCaches(bucketName).ToList();
            Assert.Contains(caches, cache => cache.Name == rapidCacheName);

            // Update
            var updateSample = new StorageControlUpdateRapidCacheSample();
            RapidCache updated = updateSample.StorageControlUpdateRapidCache(bucketName, rapidCacheId, 172800);
            Assert.Equal(rapidCacheName, updated.Name);
            Assert.Equal(172800L, updated.Ttl.Seconds);
        }
        catch
        {
            // Best-effort disable so the cache is cleaned up, without hiding the original failure.
            try
            {
                disableSample.StorageControlDisableRapidCache(bucketName, rapidCacheId, s_cleanupDisableTimeout);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Best-effort disable of Rapid Cache {rapidCacheName}: {e.Message}");
            }
            throw;
        }

        // Disable. This also cleans up the cache so that the bucket can be deleted once the
        // cache has been fully removed.
        try
        {
            RapidCache disabled = disableSample.StorageControlDisableRapidCache(bucketName, rapidCacheId, s_disableTimeout);
            Assert.Equal(rapidCacheName, disabled.Name);
        }
        catch (Exception e) when (e is TimeoutException
            || (e is RpcException rpcException && rpcException.StatusCode == StatusCode.DeadlineExceeded))
        {
            // The operation can take longer than the bounded wait, but the cache moves to the
            // "disabled" state within seconds.
            Console.WriteLine($"Disable operation for {rapidCacheName} did not complete within {s_disableTimeout}.");
            Assert.Equal("disabled", getSample.StorageControlGetRapidCache(bucketName, rapidCacheId).State);
        }
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
        Thread.Sleep(s_bucketPropagationDelay);
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
                Console.WriteLine($"Could not delete bucket {bucketName}, leaving it for stale bucket cleanup: {e.Message}");
                // Give the fixture one more best-effort attempt when it is disposed.
                _fixture.TempBucketNames.Add(bucketName);
                return;
            }
        }
    }
}
