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

using Google.Cloud.Storage.V1;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(StorageFixture))]
public class GetSoftDeleteBucketTest
{
    private readonly StorageFixture _fixture;

    public GetSoftDeleteBucketTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetSoftDeleteBucket()
    {
        GetSoftDeletedBucketSample getSoftDeletedBucketSample = new GetSoftDeletedBucketSample();
        var bucketName = _fixture.GenerateBucketName();
        var softDeleteBucket = _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        await _fixture.Client.DeleteBucketAsync(softDeleteBucket.Name);

        var softDeleted = getSoftDeletedBucketSample.GetSoftDeletedBucket(softDeleteBucket.Name, softDeleteBucket.Generation);
        Assert.Equal(softDeleteBucket.Name, softDeleted.Name);
        Assert.Equal(softDeleteBucket.Generation, softDeleted.Generation);
        Assert.NotNull(softDeleted.SoftDeleteTimeDateTimeOffset);
        Assert.NotNull(softDeleted.HardDeleteTimeDateTimeOffset);
    }
}
