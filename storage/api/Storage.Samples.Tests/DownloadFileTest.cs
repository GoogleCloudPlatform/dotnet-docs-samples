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

using System.IO;
using Xunit;

[Collection(nameof(BucketFixture))]
public class DownloadFileTest
{
    private readonly BucketFixture _bucketFixture;

    public DownloadFileTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void DownloadFile()
    {
        DownloadFileSample downloadFileSample = new DownloadFileSample();
        downloadFileSample.DownloadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName, "Download-test.txt");
        Assert.Equal(File.ReadAllText(_bucketFixture.FilePath), File.ReadAllText("Download-test.txt"));
        File.Delete("Download-test.txt");
    }
}
