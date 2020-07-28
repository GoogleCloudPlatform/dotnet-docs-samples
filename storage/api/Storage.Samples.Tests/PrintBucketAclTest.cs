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

using System.Linq;
using Xunit;

[Collection(nameof(BucketFixture))]
public class PrintBucketAclTest
{
    private readonly BucketFixture _bucketFixture;

    public PrintBucketAclTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void PrintBucketAcl()
    {
        PrintBucketAclSample printBucketAclSample = new PrintBucketAclSample();
        PrintBucketAclForUserSample printBucketAclForUserSample = new PrintBucketAclForUserSample();
        PrintBucketDefaultAclSample printBucketDefaultAclSample = new PrintBucketDefaultAclSample();
        AddBucketOwnerSample addBucketOwnerSample = new AddBucketOwnerSample();
        AddBucketDefaultOwnerSample addBucketDefaultOwnerSample = new AddBucketDefaultOwnerSample();
        RemoveBucketOwnerSample removeBucketOwnerSample = new RemoveBucketOwnerSample();
        RemoveBucketDefaultOwnerSample removeBucketDefaultOwnerSample = new RemoveBucketDefaultOwnerSample();
        string userEmail = _bucketFixture.ServiceAccountEmail;

        // print bucket acl
        var bucketAcl = printBucketAclSample.PrintBucketAcl(_bucketFixture.BucketNameGeneric);
        Assert.DoesNotContain(bucketAcl, c => c.Email == userEmail);

        // add bucket owner
        addBucketOwnerSample.AddBucketOwner(_bucketFixture.BucketNameGeneric, userEmail);

        // print bucket acl
        bucketAcl = printBucketAclSample.PrintBucketAcl(_bucketFixture.BucketNameGeneric);
        Assert.Contains(bucketAcl, c => c.Email == userEmail);

        // Make sure we print-acl-for-user shows us the user, but not all the ACLs.
        var bucketAclForUser = printBucketAclForUserSample.PrintBucketAclForUser(_bucketFixture.BucketNameGeneric, userEmail);
        Assert.Contains(bucketAclForUser, c => c.Email == userEmail && bucketAcl.Count() > bucketAclForUser.Count());

        // add default owner
        addBucketDefaultOwnerSample.AddBucketDefaultOwner(_bucketFixture.BucketNameGeneric, userEmail);

        // print default owner
        var defaultBucketAcl = printBucketDefaultAclSample.PrintBucketDefaultAcl(_bucketFixture.BucketNameGeneric);
        Assert.Contains(defaultBucketAcl, c => c.Role == "OWNER" && c.Email == userEmail);

        // remove default owner
        removeBucketDefaultOwnerSample.RemoveBucketDefaultOwner(_bucketFixture.BucketNameGeneric, userEmail);

        // remove owner.
        removeBucketOwnerSample.RemoveBucketOwner(_bucketFixture.BucketNameGeneric, userEmail);
    }
}
