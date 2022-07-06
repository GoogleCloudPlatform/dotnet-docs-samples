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

[Collection(nameof(StorageFixture))]
public class RemoveBucketDefaultOwnerTest
{
    private readonly StorageFixture _fixture;

    public RemoveBucketDefaultOwnerTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestRemoveBucketDefaultOwner()
    {
        AddBucketDefaultOwnerSample addBucketDefaultOwnerSample = new AddBucketDefaultOwnerSample();
        RemoveBucketDefaultOwnerSample removeBucketDefaultOwnerSample = new RemoveBucketDefaultOwnerSample();
        GetBucketMetadataSample getBucketMetadataSample = new GetBucketMetadataSample();

        // Add bucket default owner.
        addBucketDefaultOwnerSample.AddBucketDefaultOwner(_fixture.BucketNameGeneric, _fixture.ServiceAccountEmail);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // Remove bucket default owner.
        removeBucketDefaultOwnerSample.RemoveBucketDefaultOwner(_fixture.BucketNameGeneric, _fixture.ServiceAccountEmail);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // Get bucket metadata.
        var metadata = getBucketMetadataSample.GetBucketMetadata(_fixture.BucketNameGeneric);

        Assert.DoesNotContain(metadata.DefaultObjectAcl, acl => acl.Email == _fixture.ServiceAccountEmail && acl.Role == "OWNER");
    }
}
