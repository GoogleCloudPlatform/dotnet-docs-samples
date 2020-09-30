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
public class DownloadEncryptedFileTest
{
    private readonly BucketFixture _bucketFixture;

    public DownloadEncryptedFileTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestDownloadEncryptedFile()
    {
        DownloadEncryptedFileSample downloadEncryptedFileSample = new DownloadEncryptedFileSample();
        UploadEncryptedFileSample uploadEncryptedFileSample = new UploadEncryptedFileSample();
        GenerateEncryptionKeySample generateEncryptionKeySample = new GenerateEncryptionKeySample();
        var key = generateEncryptionKeySample.GenerateEncryptionKey();

        // upload with key
        uploadEncryptedFileSample.UploadEncryptedFile(
            key, _bucketFixture.BucketNameGeneric, _bucketFixture.FilePath,
            _bucketFixture.Collect("HelloEncryptDownload.txt"));

        //download with key
        downloadEncryptedFileSample.DownloadEncryptedFile(key, _bucketFixture.BucketNameGeneric, "HelloEncryptDownload.txt", "Hello-downloaded.txt");
        Assert.Equal(File.ReadAllText(_bucketFixture.FilePath), File.ReadAllText("Hello-downloaded.txt"));
        File.Delete("Hello-downloaded.txt");
    }
}
