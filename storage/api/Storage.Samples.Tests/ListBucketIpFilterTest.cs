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
public class ListBucketIpFilterTest
{
    private readonly StorageFixture _fixture;

    public ListBucketIpFilterTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestListBucketIpFilter()
    {
        var createIpFilter = new CreateBucketWithIpFilterSample();
        var listIpFilter = new ListBucketIpFiltersSample();

        var bucketName = _fixture.GenerateBucketName();
        createIpFilter.CreateBucketWithIpFilter(_fixture.ProjectId, bucketName);
        var bucketIpFilter = listIpFilter.ListBucketIpFilters(bucketName);

        Assert.NotNull(bucketIpFilter);
        Assert.Equal("Disabled", bucketIpFilter.Mode);
        Assert.True(bucketIpFilter.AllowAllServiceAgentAccess);
        Assert.True(bucketIpFilter.AllowCrossOrgVpcs);
        
        // Ensure the CIDR range is correctly set
        Assert.NotNull(bucketIpFilter.PublicNetworkSource);
        Assert.Contains("203.0.113.0/24", bucketIpFilter.PublicNetworkSource.AllowedIpCidrRanges);

    }
}
