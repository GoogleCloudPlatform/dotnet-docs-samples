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

using System.Linq;
using Xunit;

[Collection(nameof(StorageFixture))]
public class EnableBucketIpFilterTest
{
    private readonly StorageFixture _fixture;

    public EnableBucketIpFilterTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestEnableBucketIpFilter()
    {
        var updateSample = new EnableBucketIpFilterSample();
        var bucketName = _fixture.GenerateBucketName();
        var projectId = _fixture.ProjectId;
        string newPublicRange = "192.0.2.0/24";
        string newVpcRange = "10.0.0.0/24";
        _fixture.CreateBucket(bucketName, multiVersion: false, ipFilter: false, registerForDeletion: true);
        var updatedBucket = updateSample.EnableBucketIpFilter(projectId, bucketName);
        Assert.NotNull(updatedBucket.IpFilter);
        Assert.Equal("Enabled", updatedBucket.IpFilter.Mode);
        var publicRanges = updatedBucket.IpFilter.PublicNetworkSource?.AllowedIpCidrRanges;
        Assert.NotNull(publicRanges);
        Assert.Contains(newPublicRange, publicRanges);
        var vpcNetwork = updatedBucket.IpFilter.VpcNetworkSources?
            .FirstOrDefault(v => v.Network == $"projects/{projectId}/global/networks/default");
        Assert.NotNull(vpcNetwork);
        var vpcRanges = vpcNetwork.AllowedIpCidrRanges;
        Assert.NotNull(vpcRanges);
        Assert.Contains(newVpcRange, vpcRanges);
    }
}
