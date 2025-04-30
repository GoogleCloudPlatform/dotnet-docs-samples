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

using System;
using Xunit;

[Collection(nameof(StorageFixture))]
public class BucketSetSoftDeletePolicyTest
{
    private readonly StorageFixture _fixture;

    public BucketSetSoftDeletePolicyTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void BucketSetSoftDeletePolicy()
    {
        BucketSetSoftDeletePolicySample bucketSetSoftDeletePolicy = new BucketSetSoftDeletePolicySample();
        var bucketName = _fixture.GenerateBucketName();
        var bucketPreSetSoftDeletePolicy = _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        int retentionDurationInDays = 10;
        long retentionDurationInSeconds = (long) TimeSpan.FromDays(retentionDurationInDays).TotalSeconds;
        Assert.NotEqual(bucketPreSetSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);
        // Set soft-delete policy for the bucket.
        var bucketPostSetSoftDeletePolicy = bucketSetSoftDeletePolicy.BucketSetSoftDeletePolicy(bucketName, retentionDurationInDays);
        Assert.Equal(bucketPostSetSoftDeletePolicy.SoftDeletePolicy.RetentionDurationSeconds, retentionDurationInSeconds);
    }
}
