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

[Collection(nameof(StorageFixture))]
public class UploadEncryptedFileTest
{
    private readonly StorageFixture _fixture;

    public UploadEncryptedFileTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestUploadEncryptedFile()
    {
        DownloadEncryptedFileSample downloadEncryptedFileSample = new DownloadEncryptedFileSample();
        UploadEncryptedFileSample uploadEncryptedFileSample = new UploadEncryptedFileSample();
        GenerateEncryptionKeySample generateEncryptionKeySample = new GenerateEncryptionKeySample();
        var key = generateEncryptionKeySample.GenerateEncryptionKey();

        // upload with key
        uploadEncryptedFileSample.UploadEncryptedFile(key, _fixture.BucketNameGeneric, _fixture.FilePath,
            _fixture.Collect("HelloEncryptUpload.txt"));

        //download with key
        downloadEncryptedFileSample.DownloadEncryptedFile(key, _fixture.BucketNameGeneric, "HelloEncryptUpload.txt", "Hello-downloaded.txt");
        Assert.Equal(File.ReadAllText(_fixture.FilePath), File.ReadAllText("Hello-downloaded.txt"));
        File.Delete("Hello-downloaded.txt");
    }
}
