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
public class PrintFileAclTest
{
    private readonly StorageFixture _fixture;

    public PrintFileAclTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void PrintFileAcl()
    {
        PrintFileAclSample printFileAclSample = new PrintFileAclSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();

        string userEmail = _fixture.ServiceAccountEmail;
        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, _fixture.FilePath, _fixture.Collect("HelloAddObjectOwner.txt"));

        // print file acl
        var fileAcl = printFileAclSample.PrintObjectAcl(_fixture.BucketNameGeneric, "HelloAddObjectOwner.txt");
        Assert.DoesNotContain(fileAcl, c => c.Email == userEmail);

        // add file owner
        addFileOwnerSample.AddFileOwner(_fixture.BucketNameGeneric, "HelloAddObjectOwner.txt", userEmail);

        // print file acl
        fileAcl = printFileAclSample.PrintObjectAcl(_fixture.BucketNameGeneric, "HelloAddObjectOwner.txt");
        Assert.Contains(fileAcl, c => c.Email == userEmail);
    }
}
