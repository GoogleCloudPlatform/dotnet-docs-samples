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

// [START storage_set_bucket_default_kms_key]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class EnableDefaultKMSKeySample
{
    public Bucket EnableDefaultKMSKey(
        string projectId = "your-project-id",
        string bucketName = "your-unique-bucket-name",
        string keyLocation = "us-west1",
        string kmsKeyRing = "kms-key-ring",
        string kmsKeyName = "key-name")
    {
        // KMS Key identifier of an already created KMS key.
        // If you use the Google.Cloud.Kms.V1 library, you can construct these names using helper class CryptoKeyName.
        // var fullKeyName = new CryptoKeyName(projectId, keyLocation, kmsKeyRing, kmsKeyName).ToString();
        string keyPrefix = $"projects/{projectId}/locations/{keyLocation}";
        string fullKeyringName = $"{keyPrefix}/keyRings/{kmsKeyRing}";
        string fullKeyName = $"{fullKeyringName}/cryptoKeys/{kmsKeyName}";
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName, new GetBucketOptions { Projection = Projection.Full });
        bucket.Encryption = new Bucket.EncryptionData { DefaultKmsKeyName = fullKeyName };
        var updatedBucket = storage.UpdateBucket(bucket);
        Console.WriteLine($"Default KMS key for {bucketName} was set to {kmsKeyName}.");
        return updatedBucket;
    }
}
// [END storage_set_bucket_default_kms_key]
