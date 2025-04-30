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
public class BucketGetSoftDeletePolicyTest
{
    private readonly StorageFixture _fixture;

    public BucketGetSoftDeletePolicyTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void BucketGetSoftDeletePolicy()
    {
        BucketGetSoftDeletePolicySample getBucketSoftDeletePolicy = new BucketGetSoftDeletePolicySample();
        BucketSetSoftDeletePolicySample setSoftDeletePolicy = new BucketSetSoftDeletePolicySample();
        BucketDisableSoftDeletePolicySample disableSoftDeletePolicy = new BucketDisableSoftDeletePolicySample();
        UploadObjectFromMemorySample uploadObjectFromMemory = new UploadObjectFromMemorySample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        var bucketName = _fixture.GenerateBucketName();
        var bucketPreFetchSoftDeletePolicy = _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        var originName = _fixture.GenerateName();
        var originContent = _fixture.GenerateContent();
        uploadObjectFromMemory.UploadObjectFromMemory(bucketName, originName, originContent);
        var objectMetaData = getMetadataSample.GetMetadata(bucketName, originName);
        var bucketPostFetchSoftDeletePolicy = getBucketSoftDeletePolicy.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketPreFetchSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, bucketPostFetchSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds);
        int retentionDurationInDays = 10;
        long retentionDurationInSeconds = (long) TimeSpan.FromDays(retentionDurationInDays).TotalSeconds;
        Assert.NotEqual(bucketPostFetchSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);
        setSoftDeletePolicy.BucketSetSoftDeletePolicy(bucketName, retentionDurationInDays);
        var bucketPostSetSoftDeletePolicy = getBucketSoftDeletePolicy.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketPostSetSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);
        int disableSoftDeleteRetentionDurationInDay = 0;
        long disableSoftDeleteRetentionDurationInSeconds = (long) TimeSpan.FromDays(disableSoftDeleteRetentionDurationInDay).TotalSeconds;
        Assert.NotEqual(bucketPostSetSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, disableSoftDeleteRetentionDurationInSeconds);
        disableSoftDeletePolicy.BucketDisableSoftDeletePolicy(bucketName, disableSoftDeleteRetentionDurationInDay);
        var bucketPostDisableSoftDeletePolicy = getBucketSoftDeletePolicy.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketPostDisableSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, disableSoftDeleteRetentionDurationInSeconds);
        _fixture.Client.DeleteObject(bucketName, originName);
        var exception = Assert.Throws<GoogleApiException>(() => _fixture.Client.RestoreObject(bucketName, originName, objectMetaData.Generation.Value));
        Assert.Equal(HttpStatusCode.BadRequest, exception.HttpStatusCode);
    }
}
