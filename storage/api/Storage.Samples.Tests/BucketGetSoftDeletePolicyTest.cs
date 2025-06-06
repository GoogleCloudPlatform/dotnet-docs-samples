// Copyright 2025 Google LLC
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

using Newtonsoft.Json;
using Xunit;

[Collection(nameof(StorageFixture))]
public class BucketGetSoftDeletePolicyTest
{
    private readonly StorageFixture _fixture;

    public BucketGetSoftDeletePolicyTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestBucketGetSoftDeletePolicy()
    {
        BucketGetSoftDeletePolicySample getSample = new BucketGetSoftDeletePolicySample();
        var bucketName = _fixture.GenerateBucketName();
        var bucket = _fixture.CreateBucket(bucketName, multiVersion: false, softDelete: true, registerForDeletion: true);
        var softPolicyData = getSample.BucketGetSoftDeletePolicy(bucketName);
        var bucketSoftDeletePolicy = JsonConvert.SerializeObject(bucket.SoftDeletePolicy);
        var softDeletePolicyData = JsonConvert.SerializeObject(softPolicyData);
        Assert.Equal(bucketSoftDeletePolicy, softDeletePolicyData);
    }
}
