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
public class PrintFileAclTest
{
    private readonly BucketFixture _bucketFixture;

    public PrintFileAclTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void PrintFileAcl()
    {
        PrintFileAclSample printFileAclSample = new PrintFileAclSample();
        PrintFileAclForUserSample printFileAclForUserSample = new PrintFileAclForUserSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();
        RemoveFileOwnerSample removeFileOwnerSample = new RemoveFileOwnerSample();

        string userEmail = "230835935096-8io28ro0tvbbv612p5k6nstlaucmhnrq@developer.gserviceaccount.com";
        uploadFileSample.UploadFile(_bucketFixture.BucketName, _bucketFixture.FilePath, _bucketFixture.Collect("HelloAddObjectOwner.txt"));

        // print file acl
        var fileAcl = printFileAclSample.PrintObjectAcl(_bucketFixture.BucketName, "HelloAddObjectOwner.txt");
        Assert.DoesNotContain(fileAcl, c => c.Email == userEmail);

        // add file owner
        addFileOwnerSample.AddFileOwner(_bucketFixture.BucketName, "HelloAddObjectOwner.txt", userEmail);

        // print file acl
        fileAcl = printFileAclSample.PrintObjectAcl(_bucketFixture.BucketName, "HelloAddObjectOwner.txt");
        Assert.Contains(fileAcl, c => c.Email == userEmail);

        // Make sure we print-acl-for-user shows us the user, but not all the ACLs.
        var fileAclForUser = printFileAclForUserSample.PrintFileAclForUser(_bucketFixture.BucketName, "HelloAddObjectOwner.txt", userEmail);
        Assert.Contains(fileAcl, c => c.Email == userEmail && fileAcl.Count() > fileAclForUser.Count());

        removeFileOwnerSample.RemoveFileOwner(_bucketFixture.BucketName, "HelloAddObjectOwner.txt", userEmail);
    }
}
