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

[Collection(nameof(StorageFixture))]
public class PrintBucketAclTest
{
    private readonly StorageFixture _fixture;

    public PrintBucketAclTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestPrintBucketAcl()
    {
        PrintBucketAclSample printBucketAclSample = new PrintBucketAclSample();
        AddBucketOwnerSample addBucketOwnerSample = new AddBucketOwnerSample();
        RemoveBucketOwnerSample removeBucketOwnerSample = new RemoveBucketOwnerSample();
        string userEmail = _fixture.ServiceAccountEmail;

        // print bucket acl
        var bucketAcl = printBucketAclSample.PrintBucketAcl(_fixture.BucketNameGeneric);
        Assert.DoesNotContain(bucketAcl, c => c.Email == userEmail);

        // add bucket owner
        addBucketOwnerSample.AddBucketOwner(_fixture.BucketNameGeneric, userEmail);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // print bucket acl
        bucketAcl = printBucketAclSample.PrintBucketAcl(_fixture.BucketNameGeneric);
        Assert.Contains(bucketAcl, c => c.Email == userEmail);

        // remove owner.
        removeBucketOwnerSample.RemoveBucketOwner(_fixture.BucketNameGeneric, userEmail);
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }
}
