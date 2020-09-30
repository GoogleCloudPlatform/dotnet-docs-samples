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
using System.Net;
using Xunit;

[Collection(nameof(BucketFixture))]
public class MakePublicTest
{
    private readonly BucketFixture _bucketFixture;

    public MakePublicTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void MakePublic()
    {
        MakePublicSample makePublicSample = new MakePublicSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        uploadFileSample.UploadFile(_bucketFixture.BucketNameGeneric, _bucketFixture.FilePath, _bucketFixture.Collect("HelloMakePublic.txt"));

        var metadata = getMetadataSample.GetMetadata(_bucketFixture.BucketNameGeneric, "HelloMakePublic.txt");
        Assert.NotNull(metadata.MediaLink);

        // Before making the file public, fetching the medialink should throw an exception.
        WebClient webClient = new WebClient();
        Assert.Throws<WebException>(() => webClient.DownloadString(metadata.MediaLink));

        // Make it public and try fetching again.
        var medialink = makePublicSample.MakePublic(_bucketFixture.BucketNameGeneric, "HelloMakePublic.txt");
        var text = webClient.DownloadString(medialink);
        Assert.Equal(File.ReadAllText(_bucketFixture.FilePath), text);
    }
}
