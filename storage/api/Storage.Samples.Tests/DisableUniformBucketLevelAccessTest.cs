﻿// Copyright 2020 Google Inc.
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

[Collection(nameof(BucketFixture))]
public class DisableUniformBucketLevelAccessTest
{
    private readonly BucketFixture _bucketFixture;

    public DisableUniformBucketLevelAccessTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestDisableUniformBucketLevelAccess()
    {
        EnableUniformBucketLevelAccessSample enableUniformBucketLevelAccessSample = new EnableUniformBucketLevelAccessSample();
        DisableUniformBucketLevelAccessSample disableUniformBucketLevelAccessSample = new DisableUniformBucketLevelAccessSample();

        var bucketName = Guid.NewGuid().ToString();
        // Create bucket
        _bucketFixture.CreateBucket(bucketName);

        // Enable Uniform bucket level access.
        enableUniformBucketLevelAccessSample.EnableUniformBucketLevelAccess(bucketName);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Disable Uniform bucket level access.
        var updatedBucket = disableUniformBucketLevelAccessSample.DisableUniformBucketLevelAccess(bucketName);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        Assert.False(updatedBucket.IamConfiguration.UniformBucketLevelAccess.Enabled);
    }
}
