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

using System;
using Xunit;

[Collection(nameof(StorageFixture))]
public class GetUniformBucketLevelAccessTest
{
    private readonly StorageFixture _fixture;

    public GetUniformBucketLevelAccessTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestGetUniformBucketLevelAccess()
    {
        EnableUniformBucketLevelAccessSample enableUniformBucketLevelAccessSample = new EnableUniformBucketLevelAccessSample();
        GetUniformBucketLevelAccessSample getUniformBucketLevelAccessSample = new GetUniformBucketLevelAccessSample();

        var bucketName = Guid.NewGuid().ToString();
        // Create bucket
        _fixture.CreateBucket(bucketName);

        // Enable Uniform bucket level access.
        enableUniformBucketLevelAccessSample.EnableUniformBucketLevelAccess(bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        var uniformBucketLevelAccess = getUniformBucketLevelAccessSample.GetUniformBucketLevelAccess(bucketName);
        Assert.True(uniformBucketLevelAccess.Enabled);
    }
}
