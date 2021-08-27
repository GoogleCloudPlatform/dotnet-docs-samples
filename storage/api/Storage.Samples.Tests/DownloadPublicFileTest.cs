// Copyright 2021 Google Inc.
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
using System.Net;
using Xunit;

[Collection(nameof(BucketFixture))]
public class DownloadPublicFileTest
{
    private readonly BucketFixture _bucketFixture;

    public DownloadPublicFileTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void DownloadPublicFile()
    {
        MakePublicSample makePublicSample = new MakePublicSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        DownloadPublicFileSample downloadPublicFileSample = new DownloadPublicFileSample();

        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("HelloDownloadPublic.txt"));

        // Make it public
        makePublicSample.MakePublic(_bucketFixture.BucketNameGeneric, "HelloDownloadPublic.txt");

        // Try downloading without creds 
        try
        {
            downloadPublicFileSample.DownloadPublicFile(_bucketFixture.BucketNameGeneric, "HelloDownloadPublic.txt", "HelloDownloadPublic.txt");
            Assert.Equal(File.ReadAllText(_bucketFixture.FilePath), File.ReadAllText("HelloDownloadPublic.txt"));
        }
        finally
        {
            File.Delete("HelloDownloadPublic.txt");
        }
    }
}
