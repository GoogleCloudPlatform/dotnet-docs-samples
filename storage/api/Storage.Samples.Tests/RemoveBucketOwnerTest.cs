﻿// Copyright 2020 Google Inc.
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
public class RemoveBucketOwnerTest
{
    private readonly BucketFixture _bucketFixture;

    public RemoveBucketOwnerTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestRemoveBucketOwner()
    {
        AddBucketOwnerSample addBucketOwnerSample = new AddBucketOwnerSample();
        RemoveBucketOwnerSample removeBucketOwnerSample = new RemoveBucketOwnerSample();
        GetBucketMetadataSample getBucketMetadataSample = new GetBucketMetadataSample();

        // Add bucket owner.
        addBucketOwnerSample.AddBucketOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.ServiceAccountEmail);

        // Remove bucket owner.
        removeBucketOwnerSample.RemoveBucketOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.ServiceAccountEmail);

        // Get bucket metadata.
        var bucketMetadata = getBucketMetadataSample.GetBucketMetadata(_bucketFixture.BucketNameGeneric);
        Assert.DoesNotContain(bucketMetadata.Acl, acl => acl.Role == "OWNER" && acl.Email == _bucketFixture.ServiceAccountEmail);
    }
}
