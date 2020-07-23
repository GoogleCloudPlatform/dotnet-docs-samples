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

using Xunit;

[Collection(nameof(BucketFixture))]
public class BucketLifecycleManagementTest
{

    private readonly BucketFixture _bucketFixture;

    public BucketLifecycleManagementTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestBucketLifecycleManagement()
    {
        var enableBucketLifecycleManagementSample = new EnableBucketLifecycleManagementSample();
        var disableBucketLifecycleManagementSample = new DisableBucketLifecycleManagementSample();

        // Enable Bucket Lifecycle management.
        var bucket = enableBucketLifecycleManagementSample.EnableBucketLifecycleManagement(_bucketFixture.BucketName);
        Assert.Contains(bucket.Lifecycle.Rule, r => r.Condition.Age == 100 && r.Action.Type == "Delete");

        // Disable Bucket Lifecycle management.
        bucket = disableBucketLifecycleManagementSample.DisableBucketLifecycleManagement(_bucketFixture.BucketName);
        Assert.Null(bucket.Lifecycle);
    }
}
