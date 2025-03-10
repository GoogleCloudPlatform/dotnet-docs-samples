// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(StorageFixture))]
public class ListBucketsTest
{
    private readonly StorageFixture _fixture;

    public ListBucketsTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ListBuckets()
    {
        CreateBucketSample createBucketSample = new CreateBucketSample();
        ListBucketsSample listBucketsSample = new ListBucketsSample();
        var bucketName = Guid.NewGuid().ToString();
        _fixture.CreateBucket(bucketName);

        var buckets = listBucketsSample.ListBuckets(_fixture.ProjectId);
        Assert.Contains(buckets, c => c.Name == bucketName);
    }

    [Fact]
    public async Task SoftDeletedOnly()
    {
        ListSoftDeletedBucketsSample listSoftDeletedBucketsSample = new ListSoftDeletedBucketsSample();
        var bucketName = _fixture.GenerateBucketName();
        var softDeleteBucket = _fixture.CreateSoftDeleteBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        await _fixture.Client.DeleteBucketAsync(softDeleteBucket.Name, new DeleteBucketOptions { DeleteObjects = true });

        var actualBuckets = listSoftDeletedBucketsSample.ListSoftDeletedBuckets(_fixture.ProjectId);
        // Check the list contains the bucket we just soft-deleted.
        Assert.Contains(actualBuckets, bucket => bucket.Name == softDeleteBucket.Name && bucket.Generation == softDeleteBucket.Generation);
        // Check all the buckets in the list are soft-deleted buckets.
        Assert.All(actualBuckets, AssertSoftDeletedBucket);
    }

    // Validates that the given bucket is soft-deleted.
    private void AssertSoftDeletedBucket(Bucket b)
    {
        Assert.NotNull(b.Generation);
        Assert.NotNull(b.HardDeleteTimeDateTimeOffset);
        Assert.NotNull(b.SoftDeleteTimeDateTimeOffset);
    }
}
