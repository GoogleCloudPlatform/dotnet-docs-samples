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

// [START storage_object_csek_to_cmek]

using Google.Cloud.Storage.V1;
using System;
using System.IO;

public class ObjectCsekToCmekSample
{
    public void ObjectCsekToCmek(
        string projectId = "your-project-id",
        string bucketName = "your-unique-bucket-name",
        string objectName = "your-object-name",
        string currrentEncryptionKey = "TIbv/fjexq+VmtXzAlc63J4z5kFmWJ6NdAPQulQBT7g=",
        string keyLocation = "us-west1",
        string kmsKeyRing = "kms-key-ring",
        string kmsKeyName = "key-name")
    {
        string keyPrefix = $"projects/{projectId}/locations/{keyLocation}";
        string fullKeyringName = $"{keyPrefix}/keyRings/{kmsKeyRing}";
        string fullKeyName = $"{fullKeyringName}/cryptoKeys/{kmsKeyName}";
        var storage = StorageClient.Create();

        using var outputStream = new MemoryStream();
        storage.DownloadObject(bucketName, objectName, outputStream, new DownloadObjectOptions()
        {
            EncryptionKey = EncryptionKey.Create(Convert.FromBase64String(currrentEncryptionKey))
        });

        outputStream.Position = 0;

        storage.UploadObject(bucketName, objectName, null, outputStream, new UploadObjectOptions()
        {
            KmsKeyName = fullKeyName
        });

        Console.WriteLine($"Object {objectName} in bucket {bucketName} is now managed" +
            $" by the KMS key ${kmsKeyName}  instead of a customer-supplied encryption key");
    }
}
// [END storage_object_csek_to_cmek]
