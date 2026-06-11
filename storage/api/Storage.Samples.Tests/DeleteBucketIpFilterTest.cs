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

using Google.Cloud.Storage.V1;
using System.Linq;
using Xunit;

[Collection(nameof(StorageFixture))]
public class DeleteBucketIpFilterTest
{
    private readonly StorageFixture _fixture;

    public DeleteBucketIpFilterTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestDeleteBucketIpFilter()
    {
        var deleteSample = new DeleteBucketIpFilterSample();
        var storage = StorageClient.Create();
        var bucketName = _fixture.GenerateBucketName();
        var ipFilteredBucket = _fixture.CreateBucket(bucketName, multiVersion: false, ipFilter: true, registerForDeletion: true);
        string targetPublicIp = "0.0.0.0/0";
        string targetVpc = $"projects/{_fixture.ProjectId}/global/networks/default";
        deleteSample.DeleteBucketIpFilter(bucketName, targetPublicIp, targetVpc);
        var updatedBucket = storage.GetBucket(bucketName);
        Assert.NotNull(updatedBucket.IpFilter);
        Assert.NotNull(updatedBucket.IpFilter.PublicNetworkSource);
        Assert.DoesNotContain(targetPublicIp, updatedBucket.IpFilter.PublicNetworkSource.AllowedIpCidrRanges);
        Assert.False(updatedBucket.IpFilter?.VpcNetworkSources?.Any(v => v.Network == targetVpc) ?? false);
    }
}
