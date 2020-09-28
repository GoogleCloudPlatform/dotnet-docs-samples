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

using Google.Cloud.Storage.V1;
using System;
using Xunit;

[Collection(nameof(BucketFixture))]
public class CreateRegionalBucketTest
{
    private readonly BucketFixture _bucketFixture;

    public CreateRegionalBucketTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void CreateRegionalBucket()
    {
        CreateRegionalBucketSample createRegionalBucketSample = new CreateRegionalBucketSample();
        var bucketName = Guid.NewGuid().ToString();
        var buket = createRegionalBucketSample.CreateRegionalBucket(
            _bucketFixture.ProjectId, bucketName, _bucketFixture.TestLocation, StorageClasses.Regional);

        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
        _bucketFixture.TempBucketNames.Add(bucketName);

        Assert.Equal(buket.Location.ToLower(), _bucketFixture.KmsKeyLocation.ToLower());
        Assert.Equal("regional", buket.StorageClass.ToLower());
    }
}
