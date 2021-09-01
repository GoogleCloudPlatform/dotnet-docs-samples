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
using Xunit;

[Collection(nameof(StorageFixture))]
public class ComposeObjectTest
{
    private readonly StorageFixture _fixture;

    public ComposeObjectTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ComposeObject()
    {
        UploadFileSample uploadFileSample = new UploadFileSample();
        ComposeObjectSample composeObjectSample = new ComposeObjectSample();
        DownloadFileSample downloadFileSample = new DownloadFileSample();

        var firstObject = "HelloComposeObject.txt";
        var secondObject = "HelloComposeObjectAdditional.txt";
        var targetObject = "HelloComposedDownload.txt";

        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, "Resources/Hello.txt", _fixture.Collect(firstObject));

        uploadFileSample.UploadFile(_fixture.BucketNameGeneric, "Resources/HelloDownloadCompleteByteRange.txt", _fixture.Collect(secondObject));

        composeObjectSample.ComposeObject(_fixture.BucketNameGeneric, firstObject, secondObject, _fixture.Collect(targetObject));

        // Download the composed file
        downloadFileSample.DownloadFile(_fixture.BucketNameGeneric, targetObject, targetObject);

        // Content from both file should exists in the downloaded file
        var targetContent = File.ReadAllText(targetObject);
        Assert.Contains(File.ReadAllText("Resources/Hello.txt"), targetContent);
        Assert.Contains(File.ReadAllText("Resources/HelloDownloadCompleteByteRange.txt"), targetContent);

        File.Delete(targetObject);
    }
}
