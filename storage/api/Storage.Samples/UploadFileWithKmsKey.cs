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

// [START storage_upload_with_kms_key]

using Google.Cloud.Storage.V1;
using System;
using System.IO;

public class UploadFileWithKmsKeySample
{
    public void UploadFileWithKmsKey(
        string projectId = "your-project-id",
        string bucketName = "your-unique-bucket-name",
        string keyLocation = "us-west1",
        string kmsKeyRing = "kms-key-ring",
        string kmsKeyName = "key-name",
        string localPath = "my-local-path/my-file-name",
        string objectName = "my-file-name")
    {
        // KMS Key identifier of an already created KMS key.
        // If you use the Google.Cloud.Kms.V1 library, you can construct these names using helper class CryptoKeyName.
        // var fullKeyName = new CryptoKeyName(projectId, keyLocation, kmsKeyRing, kmsKeyName).ToString();
        string keyPrefix = $"projects/{projectId}/locations/{keyLocation}";
        string fullKeyringName = $"{keyPrefix}/keyRings/{kmsKeyRing}";
        string fullKeyName = $"{fullKeyringName}/cryptoKeys/{kmsKeyName}";

        var storage = StorageClient.Create();
        using var fileStream = File.OpenRead(localPath);
        storage.UploadObject(bucketName, objectName, null, fileStream, new UploadObjectOptions { KmsKeyName = fullKeyName });
        Console.WriteLine($"Uploaded {objectName}.");
    }
}
// [END storage_upload_with_kms_key]
