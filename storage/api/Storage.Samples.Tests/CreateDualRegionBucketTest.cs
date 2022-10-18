// Copyright 2022 Google Inc.
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
public class CreateDualRegionBucketTest
{
    private readonly StorageFixture _fixture;

    public CreateDualRegionBucketTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestCreateDualRegionBucket()
    {
        CreateDualRegionBucketSample createDualRegionBucketSample = new CreateDualRegionBucketSample();
        var bucketName = Guid.NewGuid().ToString();

        var storageBucket = createDualRegionBucketSample.CreateDualRegionBucket(_fixture.ProjectId, bucketName, "US", "US-EAST1", "US-WEST1");

        _fixture.SleepAfterBucketCreateUpdateDelete();
        _fixture.TempBucketNames.Add(bucketName);

        Assert.Equal("US", storageBucket.Location);
        Assert.Equal("dual-region", storageBucket.LocationType);
        Assert.Collection<string>(storageBucket.CustomPlacementConfig.DataLocations,
            dataLocation => Assert.Equal("US-EAST1", dataLocation),
            dataLocation => Assert.Equal("US-WEST1", dataLocation));
    }
}
