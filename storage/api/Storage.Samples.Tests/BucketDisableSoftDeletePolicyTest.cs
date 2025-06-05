// Copyright 2025 Google LLC
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

using Google;
using System.Net;
using Xunit;

[Collection(nameof(StorageFixture))]
public class BucketDisableSoftDeletePolicyTest
{
    private readonly StorageFixture _fixture;

    public BucketDisableSoftDeletePolicyTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestBucketDisableSoftDeletePolicy()
    {
        BucketDisableSoftDeletePolicySample disableSample = new BucketDisableSoftDeletePolicySample();
        UploadObjectFromMemorySample uploadObjectSample = new UploadObjectFromMemorySample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        var bucketName = _fixture.GenerateBucketName();
        var bucketWithDefaultSoftDeletePolicy = _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);

        var originName = _fixture.GenerateGuid();
        var originContent = _fixture.GenerateGuid();

        uploadObjectSample.UploadObjectFromMemory(bucketName, originName, originContent);
        var objectMetaData = getMetadataSample.GetMetadata(bucketName, originName);

        // Initializing zero with value 0 indicates that the retention duration for the bucket. 
        long zero = 0;
        Assert.NotEqual(bucketWithDefaultSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, zero);

        // To disable soft-delete policy for the bucket, set the soft delete retention duration to 0 days.
        var bucketPostDisableSoftDeletePolicy = disableSample.BucketDisableSoftDeletePolicy(bucketName, (int) zero);
        Assert.Equal(bucketPostDisableSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, zero);

        // After disabling soft-delete policy for the bucket, EffectiveTimeRaw property will be null.
        Assert.Null(bucketPostDisableSoftDeletePolicy.SoftDeletePolicy.EffectiveTimeRaw);

        // After disabling soft-delete policy for the bucket, objects in the bucket can not be restored.
        var exception = Assert.Throws<GoogleApiException>(() => _fixture.Client.RestoreObject(bucketName, originName, objectMetaData.Generation.Value));
        Assert.Equal(HttpStatusCode.BadRequest, exception.HttpStatusCode);
    }
}
