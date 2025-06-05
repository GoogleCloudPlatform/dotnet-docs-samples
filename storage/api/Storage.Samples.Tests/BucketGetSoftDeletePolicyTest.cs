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
    public void TestBucketGetSoftDeletePolicy()
    {
        BucketGetSoftDeletePolicySample getSample = new BucketGetSoftDeletePolicySample();
        BucketSetSoftDeletePolicySample setSample = new BucketSetSoftDeletePolicySample();
        BucketDisableSoftDeletePolicySample disableSample = new BucketDisableSoftDeletePolicySample();

        var bucketName = _fixture.GenerateBucketName();
        var bucketWithDefaultSoftDeletePolicy = _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        var retrievedBucket = getSample.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketWithDefaultSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retrievedBucket.SoftDeletePolicy.RetentionDurationSeconds);

        int retentionDurationInDays = 10;
        long retentionDurationInSeconds = (long) TimeSpan.FromDays(retentionDurationInDays).TotalSeconds;

        // Set soft-delete policy for the bucket with a retention duration of 10 days.
        setSample.BucketSetSoftDeletePolicy(bucketName, retentionDurationInDays);
        var bucketPostSetSoftDeletePolicy = getSample.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketPostSetSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);

        // Initializing zero with value 0 indicates that the retention duration for the bucket. 
        long zero = 0;
        // Disable soft-delete policy for the bucket by setting the retention duration to 0 days.
        disableSample.BucketDisableSoftDeletePolicy(bucketName, (int) zero);
        var bucketPostDisableSoftDeletePolicy = getSample.BucketGetSoftDeletePolicy(bucketName);
        Assert.Equal(bucketPostDisableSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, zero);
    }
}
