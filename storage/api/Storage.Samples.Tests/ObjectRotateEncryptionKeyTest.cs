﻿// Copyright 2021 Google Inc.
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
public class ObjectRotateEncryptionKeyTest
{
    private readonly StorageFixture _fixture;

    public ObjectRotateEncryptionKeyTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ObjectRotateEncryptionKey()
    {
        GenerateEncryptionKeySample generateEncryptionKeySample = new GenerateEncryptionKeySample();
        UploadEncryptedFileSample uploadEncryptedFileSample = new UploadEncryptedFileSample();
        DownloadEncryptedFileSample downloadEncryptedFileSample = new DownloadEncryptedFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();
        ObjectRotateEncryptionKeySample objectRotateEncryptionKeySample = new ObjectRotateEncryptionKeySample();

        // Upload with key
        var objectName = "HelloObjectRotateEncryptionKey.txt";
        string currentKey = generateEncryptionKeySample.GenerateEncryptionKey();
        string newKey = generateEncryptionKeySample.GenerateEncryptionKey();

        uploadEncryptedFileSample.UploadEncryptedFile(currentKey, _fixture.BucketNameGeneric, _fixture.FilePath, _fixture.Collect(objectName));

        // Rotate key
        objectRotateEncryptionKeySample.ObjectRotateEncryptionKey(_fixture.BucketNameGeneric, objectName, currentKey, newKey);

        // Download with new key to verify key has changed
        downloadEncryptedFileSample.DownloadEncryptedFile(newKey, _fixture.BucketNameGeneric, objectName, "Downloaded-encrypted-object.txt");
        Assert.Equal(File.ReadAllText(_fixture.FilePath), File.ReadAllText("Downloaded-encrypted-object.txt"));
        File.Delete("Downloaded-encrypted-object.txt");
    }
}
