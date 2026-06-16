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
public class ListBucketsWithIpFilterStatusTest
{
    private readonly StorageFixture _fixture;

    public ListBucketsWithIpFilterStatusTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestListBucketsWithIpFilterStatus()
    {
        var listBucketsWithIpFilter = new ListBucketsWithIpFilterStatusSample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName, multiVersion: false, ipFilter: true, registerForDeletion: true);
        var buckets = listBucketsWithIpFilter.ListBucketsWithIpFilterStatus(_fixture.ProjectId);
        var targetBucket = buckets.FirstOrDefault(b => b.Name == bucketName);
        Assert.NotNull(targetBucket);
        Assert.Equal("Disabled", targetBucket.IpFilter?.Mode);
    }
}
