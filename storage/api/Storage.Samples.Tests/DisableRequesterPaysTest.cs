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

[Collection(nameof(StorageFixture))]
public class DisableRequesterPaysTest
{
    private readonly StorageFixture _fixture;

    public DisableRequesterPaysTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestDisableRequesterPays()
    {
        EnableRequesterPaysSample enableRequesterPaysSample = new EnableRequesterPaysSample();
        DisableRequesterPaysSample disableRequesterPaysSample = new DisableRequesterPaysSample();

        var bucketName = Guid.NewGuid().ToString();
        // Create bucket
        _fixture.CreateBucket(bucketName);

        // Enable request pay.
        enableRequesterPaysSample.EnableRequesterPays(bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // Disable request pay.
        var bucket = disableRequesterPaysSample.DisableRequesterPays(_fixture.ProjectId, bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();
        Assert.False(bucket.Billing?.RequesterPays);
    }
}
