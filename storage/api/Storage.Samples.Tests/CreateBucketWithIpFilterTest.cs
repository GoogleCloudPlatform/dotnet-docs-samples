// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Xunit;

[Collection(nameof(StorageFixture))]
public class CreateBucketWithIpFilterTest
{
    private readonly StorageFixture _fixture;

    public CreateBucketWithIpFilterTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestCreateBucketWithIpFilter()
    {
        var createIpFilter = new CreateBucketWithIpFilterSample();

        var bucketName = _fixture.GenerateBucketName();
        var bucket = createIpFilter.CreateBucketWithIpFilter(_fixture.ProjectId, bucketName);
        _fixture.SleepAfterBucketCreateUpdateDelete();
        _fixture.TempBucketNames.Add(bucket.Name);
        Assert.Equal("Disabled", bucket.IpFilter.Mode);
        Assert.True(bucket.IpFilter.AllowAllServiceAgentAccess);
        Assert.True(bucket.IpFilter.AllowCrossOrgVpcs);
        Assert.Contains("203.0.113.0/24", bucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges);
    }
}
