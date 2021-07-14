// Copyright 2021 Google Inc.
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
public class BucketDisableVersioningTest
{
    private readonly BucketFixture _bucketFixture;

    public BucketDisableVersioningTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void BucketDisableVersioning()
    {
        BucketEnableVersioningSample bucketEnableVersioningSample = new BucketEnableVersioningSample();
        BucketDisableVersioningSample bucketDisableVersioningSample = new BucketDisableVersioningSample();

        // Versioning is disabled by default, so Enable versioning
        bucketEnableVersioningSample.BucketEnableVersioning(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Disable versioning
        var bucket = bucketDisableVersioningSample.BucketDisableVersioning(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
        Assert.False(bucket.Versioning.Enabled);
    }
}
