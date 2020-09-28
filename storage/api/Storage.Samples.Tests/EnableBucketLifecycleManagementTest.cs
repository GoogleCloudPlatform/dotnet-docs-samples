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
public class EnableBucketLifecycleManagementTest
{
    private readonly BucketFixture _bucketFixture;

    public EnableBucketLifecycleManagementTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestEnableBucketLifecycleManagement()
    {
        var enableBucketLifecycleManagementSample = new EnableBucketLifecycleManagementSample();
        var disableBucketLifecycleManagementSample = new DisableBucketLifecycleManagementSample();

        // Enable bucket lifecycle management.
        var bucket = enableBucketLifecycleManagementSample.EnableBucketLifecycleManagement(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
        Assert.Contains(bucket.Lifecycle.Rule, r => r.Condition.Age == 100 && r.Action.Type == "Delete");

        // Disable bucket lifecycle management.
        disableBucketLifecycleManagementSample.DisableBucketLifecycleManagement(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
    }
}
