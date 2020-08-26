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

using System.Linq;
using Xunit;

[Collection(nameof(BucketFixture))]
public class PrintFileAclForUserTest
{
    private readonly BucketFixture _bucketFixture;

    public PrintFileAclForUserTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestPrintFileAclForUser()
    {
        PrintFileAclSample printFileAclSample = new PrintFileAclSample();
        PrintFileAclForUserSample printFileAclForUserSample = new PrintFileAclForUserSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();
        RemoveFileOwnerSample removeFileOwnerSample = new RemoveFileOwnerSample();

        string userEmail = _bucketFixture.ServiceAccountEmail;
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("HelloAddObjectOwner.txt"));

        // add file owner
        addFileOwnerSample.AddFileOwner(_bucketFixture.BucketNameGeneric, "HelloAddObjectOwner.txt", userEmail);

        // print file acl
        var fileAcl = printFileAclSample.PrintObjectAcl(_bucketFixture.BucketNameGeneric, "HelloAddObjectOwner.txt");

        // Make sure we print-acl-for-user shows us the user, but not all the ACLs.
        var fileAclForUser = printFileAclForUserSample.PrintFileAclForUser(_bucketFixture.BucketNameGeneric, "HelloAddObjectOwner.txt", userEmail);
        Assert.Contains(fileAcl, c => c.Email == userEmail && fileAcl.Count() > fileAclForUser.Count());

        removeFileOwnerSample.RemoveFileOwner(_bucketFixture.BucketNameGeneric, "HelloAddObjectOwner.txt", userEmail);
    }
}
