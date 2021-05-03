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

// [START storage_rotate_encryption_key]

using Google.Cloud.Storage.V1;
using System;
using System.IO;

public class ObjectRotateEncryptionKeySample
{
    public void ObjectRotateEncryptionKey(
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name",
        string currrentEncryptionKey = "TIbv/fjexq+VmtXzAlc63J4z5kFmWJ6NdAPQulQBT7g=",
        string newEncryptionKey = "ARbt/judaq+VmtXzAsc83J4z5kFmWJ6NdAPQuleuB7g=")
    {
        var storage = StorageClient.Create();

        using var outputStream = new MemoryStream();
        storage.DownloadObject(bucketName, objectName, outputStream, new DownloadObjectOptions()
            {
                EncryptionKey = EncryptionKey.Create(Convert.FromBase64String(currrentEncryptionKey))
            });

        outputStream.Position = 0;

        storage.UploadObject(bucketName, objectName, null, outputStream, new UploadObjectOptions()
        {
            EncryptionKey = EncryptionKey.Create(Convert.FromBase64String(newEncryptionKey))
        });

        Console.WriteLine($"Rotated encryption key for object  {objectName} in bucket {bucketName}");
    }
}
// [END storage_rotate_encryption_key]
