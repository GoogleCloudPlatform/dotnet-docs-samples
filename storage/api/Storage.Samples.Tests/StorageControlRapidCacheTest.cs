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
public class StorageControlRapidCacheTest : IDisposable
{
    private const string RapidCacheType = "rapid-cache-ultra";

    // Wait for the new bucket's metadata to propagate before creating the cache.
    private static readonly TimeSpan s_bucketPropagationDelay = TimeSpan.FromSeconds(20);

    // The disable operation only completes once the cache has been fully removed, which can
    // take more than an hour, so the test bounds the wait with the LRO poll settings.
    private static readonly TimeSpan s_disableTimeout = TimeSpan.FromMinutes(10);

    // Short wait used for the best-effort disable during teardown, so that cleanup never hangs.
    private static readonly TimeSpan s_cleanupDisableTimeout = TimeSpan.FromSeconds(1);

    // Retry bucket deletion for up to 2 minutes while the disabled cache is being removed.
    private static readonly TimeSpan s_bucketDeleteTimeout = TimeSpan.FromSeconds(120);
    private static readonly TimeSpan s_bucketDeleteInterval = TimeSpan.FromSeconds(15);

    private readonly StorageFixture _fixture;
    private readonly string _zone;

    // The Rapid Cache ID must be the same as the zone.
    private readonly string _rapidCacheId;

    // Set by the test so that Dispose can clean up whatever was created.
    private string _bucketName;
    private bool _cacheActive;

    public StorageControlRapidCacheTest(StorageFixture fixture)
    {
        _fixture = fixture;
        _zone = Environment.GetEnvironmentVariable("RAPID_CACHE_ZONE") ?? "us-central1-a";
        _rapidCacheId = _zone;
    }

    // Rapid Cache requires an allowlisted project and the bucket can't be deleted until the
    // disabled cache has been fully removed (which can take more than 30 minutes), so this
    // test is excluded from CI, following the Anywhere Cache samples which have no CI tests.
    [Fact(Skip = "b/482946642: Rapid Cache requires an allowlisted project; run manually.")]
    public void TestStorageControlRapidCacheLifecycle()
    {
        _bucketName = _fixture.GenerateBucketName();
        CreateRegionalHnsBucket(_bucketName);

        string rapidCacheName = RapidCacheName.FormatProjectBucketRapidCache("_", _bucketName, _rapidCacheId);

        // Create
        var createSample = new StorageControlCreateRapidCacheSample();
        RapidCache created = createSample.StorageControlCreateRapidCache(_bucketName, _zone);
        _cacheActive = true;
        Assert.Equal(rapidCacheName, created.Name);
        Assert.Equal(RapidCacheType, created.CacheType);

        // Get
        var getSample = new StorageControlGetRapidCacheSample();
        RapidCache fetched = getSample.StorageControlGetRapidCache(_bucketName, _rapidCacheId);
        Assert.Equal(rapidCacheName, fetched.Name);
        Assert.Equal(_zone, fetched.Zone);
        Assert.Equal("running", fetched.State);
        Assert.Equal(RapidCacheType, fetched.CacheType);

        // List
        var listSample = new StorageControlListRapidCachesSample();
        var caches = listSample.StorageControlListRapidCaches(_bucketName).ToList();
        Assert.Contains(caches, cache => cache.Name == rapidCacheName);

        // Update
        var updateSample = new StorageControlUpdateRapidCacheSample();
        RapidCache updated = updateSample.StorageControlUpdateRapidCache(_bucketName, _rapidCacheId, 172800);
        Assert.Equal(rapidCacheName, updated.Name);
        Assert.Equal(172800L, updated.Ttl.Seconds);

        // Disable. The operation usually takes longer than the bounded wait, so a timeout is
        // expected; the cache itself moves to the "disabled" state within seconds.
        var disableSample = new StorageControlDisableRapidCacheSample();
        RapidCache disabled = null;
        Exception disableException = Record.Exception(
            () => disabled = disableSample.StorageControlDisableRapidCache(_bucketName, _rapidCacheId, s_disableTimeout));
        Assert.True(disableException is null || IsTimeout(disableException), disableException?.ToString());
        _cacheActive = false;
        if (disableException is null)
        {
            Assert.Equal(rapidCacheName, disabled.Name);
        }
        else
        {
            Assert.Equal("disabled", getSample.StorageControlGetRapidCache(_bucketName, _rapidCacheId).State);
        }
    }

    // Disables the cache if the test left it active, then deletes the bucket. Both steps are
    // best-effort so that teardown never hides the original test failure.
    public void Dispose()
    {
        if (_bucketName is null)
        {
            return;
        }
        if (_cacheActive)
        {
            DisableRapidCacheBestEffort(_bucketName, _rapidCacheId);
        }
        DeleteBucketBestEffort(_bucketName);
    }

    private static bool IsTimeout(Exception e) =>
        e is TimeoutException || (e is RpcException rpcException && rpcException.StatusCode == StatusCode.DeadlineExceeded);

    private static void DisableRapidCacheBestEffort(string bucketName, string rapidCacheId)
    {
        try
        {
            new StorageControlDisableRapidCacheSample().StorageControlDisableRapidCache(bucketName, rapidCacheId, s_cleanupDisableTimeout);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Best-effort disable of Rapid Cache {rapidCacheId} in bucket {bucketName}: {e.Message}");
        }
    }

    // Rapid Cache Ultra requires a regional, hierarchical namespace enabled bucket in the
    // region of the cache zone.
    private void CreateRegionalHnsBucket(string bucketName)
    {
        int lastHyphen = _zone.LastIndexOf('-');
        string region = lastHyphen > 0 ? _zone.Substring(0, lastHyphen) : _zone;
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
        // Register the bucket right away so the fixture cleans it up even if the test aborts.
        _fixture.TempBucketNames.Add(bucketName);
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
                // The bucket is registered in the fixture, which makes one more best-effort
                // attempt to delete it when it is disposed.
                Console.WriteLine($"Could not delete bucket {bucketName}, leaving it for stale bucket cleanup: {e.Message}");
                return;
            }
        }
    }
}
