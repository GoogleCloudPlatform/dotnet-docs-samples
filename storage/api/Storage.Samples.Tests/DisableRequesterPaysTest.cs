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
public class DisableRequesterPaysTest
{
    private readonly BucketFixture _bucketFixture;

    public DisableRequesterPaysTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestDisableRequesterPays()
    {
        CreateBucketSample createBucketSample = new CreateBucketSample();
        GetRequesterPaysStatusSample getRequesterPaysStatusSample = new GetRequesterPaysStatusSample();
        EnableRequesterPaysSample enableRequesterPaysSample = new EnableRequesterPaysSample();
        DisableRequesterPaysSample disableRequesterPaysSample = new DisableRequesterPaysSample();

        var bucketName = Guid.NewGuid().ToString();
        // Create bucket
        createBucketSample.CreateBucket(_bucketFixture.ProjectId, bucketName);
        _bucketFixture.SleepAfterBucketCreateDelete();
        _bucketFixture.TempBucketNames.Add(bucketName);

        // Enable request pay.
        enableRequesterPaysSample.EnableRequesterPays(bucketName);

        // Disable request pay.
        var bucket = disableRequesterPaysSample.DisableRequesterPays(_bucketFixture.ProjectId, bucketName);
        Assert.False(bucket.Billing?.RequesterPays);
    }
}
