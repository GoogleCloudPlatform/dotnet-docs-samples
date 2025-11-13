// Copyright 2025 Google LLC
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

using System.Linq;
using Xunit;

[Collection(nameof(StorageFixture))]
public class ListBucketsWithPartialSuccessTest
{
    private readonly StorageFixture _fixture;

    public ListBucketsWithPartialSuccessTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ListBucketsWithPartialSuccess()
    {
        ListBucketsWithPartialSuccessSample partialSample = new ListBucketsWithPartialSuccessSample();
        var bucketName = _fixture.GenerateBucketName();
        _fixture.CreateBucket(bucketName: bucketName, location: "US", storageClass: "MULTI_REGIONAL");

        var buckets = partialSample.ListBucketsWithPartialSuccess(_fixture.ProjectId);

        Assert.Contains(buckets.Reachable, c => c.Name == bucketName);

        if (buckets.Unreachable.Any())
        {
            // This indicates that the environment had unreachable buckets.
            // We don't assert on the count to avoid flaky tests.
        }
    }
}
