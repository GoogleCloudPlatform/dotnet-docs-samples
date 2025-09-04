// Copyright 2025 Google LLC
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
using Xunit;

[Collection(nameof(StorageFixture))]
public class StorageControlGetAnywhereCacheTest
{
    private readonly StorageFixture _fixture;

    public StorageControlGetAnywhereCacheTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestStorageControlGetAnywhereCache()
    {
        StorageControlCreateAnywhereCacheSample createCacheSample = new StorageControlCreateAnywhereCacheSample();

        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, "us");

        var createdCache = createCacheSample.StorageControlCreateAnywhereCache(bucketName, "us-west1-a");

        StorageControlGetAnywhereCacheSample getCacheSample = new StorageControlGetAnywhereCacheSample();
        var getCache = getCacheSample.StorageControlGetAnywhereCache(bucketName, createdCache.Result.Zone);

        Assert.Equal(createdCache.Result.Name, getCache.Name);
    }
}
