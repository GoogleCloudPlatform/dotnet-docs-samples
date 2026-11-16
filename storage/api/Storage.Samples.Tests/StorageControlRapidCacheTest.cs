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

using Google.Cloud.Storage.Control.V2;
using Google.LongRunning;
using System;
using System.Linq;
using System.Threading;
using Xunit;
using Bucket = Google.Apis.Storage.v1.Data.Bucket;

namespace Storage.Samples.Tests;

[Collection(nameof(StorageFixture))]
public class StorageControlRapidCacheTest : IDisposable
{
    private const string RapidCacheType = "rapid-cache-ultra";

    // The cache moves to the "disabled" state within seconds of the disable request, so the
    // test polls for that state for up to 2 minutes.
    private const int StatePollAttempts = 12;
    private static readonly TimeSpan s_statePollInterval = TimeSpan.FromSeconds(10);

    private readonly StorageFixture _fixture;
    private readonly string _zone;

    // The Rapid Cache ID is the same as the zone.
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
    // disabled cache has been fully removed (which can take more than an hour), so this
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

        // Disable. The operation only completes once the cache has been fully removed, so the
        // test checks for the "disabled" state instead of waiting for the operation.
        var disableSample = new StorageControlDisableRapidCacheSample();
        Operation<RapidCache, DisableRapidCacheMetadata> operation =
            disableSample.StorageControlDisableRapidCache(_bucketName, _rapidCacheId);
        _cacheActive = false;
        Assert.False(string.IsNullOrEmpty(operation.Name));
        Assert.Equal("disabled", WaitForState(getSample, "disabled"));
    }

    // Disables the cache if the test left it active. This is best-effort so that teardown
    // never hides the original test failure. The bucket is registered in the fixture, which
    // makes a best-effort attempt to delete it when it is disposed. That attempt usually fails
    // because the bucket can't be deleted until the disabled cache has been fully removed, so
    // the bucket may need to be deleted manually later.
    public void Dispose()
    {
        if (_bucketName is null || !_cacheActive)
        {
            return;
        }
        try
        {
            new StorageControlDisableRapidCacheSample().StorageControlDisableRapidCache(_bucketName, _rapidCacheId);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Best-effort disable of Rapid Cache {_rapidCacheId} in bucket {_bucketName}: {e.Message}");
        }
    }

    // Polls the cache until it reaches the expected state or the attempts run out, and
    // returns the last observed state.
    private string WaitForState(StorageControlGetRapidCacheSample getSample, string expectedState)
    {
        string state = null;
        for (int attempt = 1; attempt <= StatePollAttempts; attempt++)
        {
            state = getSample.StorageControlGetRapidCache(_bucketName, _rapidCacheId).State;
            if (state == expectedState || attempt == StatePollAttempts)
            {
                break;
            }
            Thread.Sleep(s_statePollInterval);
        }
        return state;
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
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }
}
