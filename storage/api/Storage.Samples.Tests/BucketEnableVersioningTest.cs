﻿// Copyright 2021 Google Inc.
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
public class BucketEnableVersioningTest
{
    private readonly StorageFixture _fixture;

    public BucketEnableVersioningTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void BucketEnableVersioning()
    {
        BucketEnableVersioningSample bucketEnableVersioningSample = new BucketEnableVersioningSample();
        BucketDisableVersioningSample bucketDisableVersioningSample = new BucketDisableVersioningSample();

        // Enable versioning
        var bucket = bucketEnableVersioningSample.BucketEnableVersioning(_fixture.BucketNameGeneric);
        _fixture.SleepAfterBucketCreateUpdateDelete();
        Assert.True(bucket.Versioning.Enabled);

        // Then disable versioning
        bucketDisableVersioningSample.BucketDisableVersioning(_fixture.BucketNameGeneric);
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }
}
