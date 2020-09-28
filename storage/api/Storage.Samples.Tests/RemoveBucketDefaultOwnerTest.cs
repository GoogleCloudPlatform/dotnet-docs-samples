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

using Xunit;

[Collection(nameof(BucketFixture))]
public class RemoveBucketDefaultOwnerTest
{
    private readonly BucketFixture _bucketFixture;

    public RemoveBucketDefaultOwnerTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestRemoveBucketDefaultOwner()
    {
        AddBucketDefaultOwnerSample addBucketDefaultOwnerSample = new AddBucketDefaultOwnerSample();
        RemoveBucketDefaultOwnerSample removeBucketDefaultOwnerSample = new RemoveBucketDefaultOwnerSample();
        GetBucketMetadataSample getBucketMetadataSample = new GetBucketMetadataSample();

        // Add bucket default owner.
        addBucketDefaultOwnerSample.AddBucketDefaultOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.ServiceAccountEmail);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Remove bucket default owner.
        removeBucketDefaultOwnerSample.RemoveBucketDefaultOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.ServiceAccountEmail);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Get bucket metadata.
        var metadata = getBucketMetadataSample.GetBucketMetadata(_bucketFixture.BucketNameGeneric);

        Assert.DoesNotContain(metadata.DefaultObjectAcl, acl => acl.Email == _bucketFixture.ServiceAccountEmail && acl.Role == "OWNER");
    }
}
