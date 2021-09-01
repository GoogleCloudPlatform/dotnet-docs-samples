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
public class PrintFileAclForUserTest
{
    private readonly StorageFixture _fixture;

    public PrintFileAclForUserTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestPrintFileAclForUser()
    {
        PrintFileAclForUserSample printFileAclForUserSample = new PrintFileAclForUserSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();

        string userEmail = _fixture.ServiceAccountEmail;
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, _fixture.FilePath, _fixture.Collect("HelloAddObjectOwnerForUser.txt"));

        // add file owner
        addFileOwnerSample.AddFileOwner(_fixture.BucketNameGeneric, "HelloAddObjectOwnerForUser.txt", userEmail);

        // Make sure we print-acl-for-user shows us the user, but not all the ACLs.
        var fileAclForUser = printFileAclForUserSample.PrintFileAclForUser(_fixture.BucketNameGeneric, "HelloAddObjectOwnerForUser.txt", userEmail);
        Assert.All(fileAclForUser, c => Assert.Equal(c.Email, userEmail));
    }
}
