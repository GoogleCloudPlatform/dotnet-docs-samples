// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Xunit;

[Collection(nameof(StorageFixture))]
public class GetBucketIpFilterTest
{
    private readonly StorageFixture _fixture;

    public GetBucketIpFilterTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestGetBucketIpFilter()
    {
        var getIpFilter = new GetBucketIpFilterSample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, ipFilter: true, registerForDeletion: true);
        var bucketIpFilter = getIpFilter.GetBucketIpFilter(bucketName);
        Assert.NotNull(bucketIpFilter);
        Assert.Equal("Disabled", bucketIpFilter.Mode);
        Assert.False(bucketIpFilter.AllowAllServiceAgentAccess);
        Assert.False(bucketIpFilter.AllowCrossOrgVpcs);
        Assert.NotNull(bucketIpFilter.PublicNetworkSource);
        Assert.NotNull(bucketIpFilter.PublicNetworkSource.AllowedIpCidrRanges);
        Assert.Contains("203.0.113.0/24", bucketIpFilter.PublicNetworkSource.AllowedIpCidrRanges);
        Assert.NotNull(bucketIpFilter.VpcNetworkSources);
    }
}
