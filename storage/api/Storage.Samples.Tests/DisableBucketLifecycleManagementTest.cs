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

[Collection(nameof(StorageFixture))]
public class DisableBucketLifecycleManagementTest
{
    private readonly StorageFixture _fixture;

    public DisableBucketLifecycleManagementTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestDisableBucketLifecycleManagement()
    {
        EnableBucketLifecycleManagementSample enableBucketLifecycleManagementSample = new EnableBucketLifecycleManagementSample();
        DisableBucketLifecycleManagementSample disableBucketLifecycleManagementSample = new DisableBucketLifecycleManagementSample();

        // Enable bucket lifecycle management.
        enableBucketLifecycleManagementSample.EnableBucketLifecycleManagement(_fixture.BucketNameGeneric);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // Disable bucket lifecycle management.
        var bucket = disableBucketLifecycleManagementSample.DisableBucketLifecycleManagement(_fixture.BucketNameGeneric);
        _fixture.SleepAfterBucketCreateUpdateDelete();
        Assert.Null(bucket.Lifecycle);
    }
}
