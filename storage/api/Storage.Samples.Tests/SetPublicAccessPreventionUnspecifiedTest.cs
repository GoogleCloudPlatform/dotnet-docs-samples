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

using System;
using Xunit;

[Collection(nameof(StorageFixture))]
public class SetPublicAccessPreventionUnspecifiedTest
{
    private readonly StorageFixture _fixture;

    public SetPublicAccessPreventionUnspecifiedTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1508")]
    public void TestSetPublicAccessPreventionUnspecified()
    {
        SetPublicAccessPreventionUnspecifiedSample setPublicAccessPreventionUnspecifiedSample = new SetPublicAccessPreventionUnspecifiedSample();

        var bucketName = Guid.NewGuid().ToString();
        // Create bucket
        _fixture.CreateBucket(bucketName);

        // Set public access prevention to unspecified.
        var updatedBucket = setPublicAccessPreventionUnspecifiedSample.SetPublicAccessPreventionUnspecified(bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        Assert.Equal("unspecified", updatedBucket.IamConfiguration.PublicAccessPrevention);
    }
}
