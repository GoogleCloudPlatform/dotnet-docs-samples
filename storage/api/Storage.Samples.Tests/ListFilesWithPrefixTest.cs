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
public class ListFilesWithPrefixTest
{
    private readonly BucketFixture _bucketFixture;

    public ListFilesWithPrefixTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void ListFilesWithPrefix()
    {
        ListFilesWithPrefixSample listFilesWithPrefixSample = new ListFilesWithPrefixSample();
        UploadFileSample uploadFileSample = new UploadFileSample();

        // Upload 4 files.
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("a/1.txt"));
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("a/2.txt"));
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("b/2.txt"));
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("a/b/3.txt"));

        var files = listFilesWithPrefixSample.ListFilesWithPrefix(_bucketFixture.BucketNameGeneric, "a/", null).ToList();
        Assert.Contains(files, c => c.Name == "a/1.txt");
        Assert.Contains(files, c => c.Name == "a/2.txt");
        Assert.Contains(files, c => c.Name == "a/b/3.txt");
    }
}
