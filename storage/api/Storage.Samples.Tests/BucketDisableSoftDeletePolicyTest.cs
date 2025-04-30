// Copyright 2025 Google Inc.
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
using System;
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
    public void BucketDisableSoftDeletePolicy()
    {
        BucketDisableSoftDeletePolicySample bucketDisableSoftDeletePolicy = new BucketDisableSoftDeletePolicySample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        var bucketName = _fixture.GenerateBucketName();
        var bucketPreDisableSoftDeletePolicy = _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        var originName = _fixture.GenerateName();
        var originContent = _fixture.GenerateContent();
        uploadObjectFromMemory.UploadObjectFromMemory(bucketName, originName, originContent);
        var objectMetaData = getMetadataSample.GetMetadata(bucketName, originName);
        int retentionDurationInDay = 0;
        long retentionDurationInSeconds = (long) TimeSpan.FromDays(retentionDurationInDay).TotalSeconds;
        Assert.NotEqual(bucketPreDisableSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);
        // To disable soft-delete policy for the bucket, set the soft delete retention duration to 0.
        var bucketPostDisableSoftDeletePolicy = bucketDisableSoftDeletePolicy.BucketDisableSoftDeletePolicy(bucketName, retentionDurationInDay);
        Assert.Equal(bucketPostDisableSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);
        _fixture.Client.DeleteObject(bucketName, originName);
        var exception = Assert.Throws<GoogleApiException>(() => _fixture.Client.RestoreObject(bucketName, originName, objectMetaData.Generation.Value));
        Assert.Equal(HttpStatusCode.BadRequest, exception.HttpStatusCode);
    }
}
