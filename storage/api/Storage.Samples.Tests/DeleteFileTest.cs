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
public class DeleteFileTest
{
    private readonly BucketFixture _bucketFixture;

    public DeleteFileTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void DeleteFile()
    {
        UploadFileSample uploadFileSample = new UploadFileSample();
        DeleteFileSample deleteFileSample = new DeleteFileSample();
        ListFilesSample listFilesSample = new ListFilesSample();
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, "DeleteTest.txt");
        deleteFileSample.DeleteFile(_bucketFixture.BucketNameGeneric, "DeleteTest.txt");
        var files = listFilesSample.ListFiles(_bucketFixture.BucketNameGeneric).ToList();
        Assert.DoesNotContain(files, c => c.Name == "DeleteTest.txt");
    }
}
