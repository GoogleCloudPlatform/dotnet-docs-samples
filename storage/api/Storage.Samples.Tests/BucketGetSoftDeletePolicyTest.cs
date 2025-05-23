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

using System;
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
        var bucketName = _fixture.GenerateBucketName();
        var bucketPreFetchSoftDeletePolicy = _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        var bucketPostFetchSoftDeletePolicy = getBucketSoftDeletePolicy.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketPreFetchSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, bucketPostFetchSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds);
        int retentionDurationInDays = 10;
        long retentionDurationInSeconds = (long) TimeSpan.FromDays(retentionDurationInDays).TotalSeconds;
        Assert.NotEqual(bucketPostFetchSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);
        // Set soft-delete policy for the bucket with a retention duration of 10 days.
        setSoftDeletePolicy.BucketSetSoftDeletePolicy(bucketName, retentionDurationInDays);
        var bucketPostSetSoftDeletePolicy = getBucketSoftDeletePolicy.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketPostSetSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);
        int disableSoftDeleteRetentionDurationInDays = 0;
        long disableSoftDeleteRetentionDurationInSeconds = (long) TimeSpan.FromDays(disableSoftDeleteRetentionDurationInDays).TotalSeconds;
        Assert.NotEqual(bucketPostSetSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, disableSoftDeleteRetentionDurationInSeconds);
        // Disable soft-delete policy for the bucket by setting the retention duration to 0 days.
        disableSoftDeletePolicy.BucketDisableSoftDeletePolicy(bucketName, disableSoftDeleteRetentionDurationInDays);
        var bucketPostDisableSoftDeletePolicy = getBucketSoftDeletePolicy.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketPostDisableSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, disableSoftDeleteRetentionDurationInSeconds);
    }
}
