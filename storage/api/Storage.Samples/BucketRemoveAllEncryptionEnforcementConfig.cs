// Copyright 2026 Google LLC
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

// [START storage_remove_all_encryption_enforcement_config]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class BucketRemoveAllEncryptionEnforcementConfigSample
{
    /// <summary>
    /// Remove all encryption enforcement configurations from the bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    public Bucket BucketRemoveAllEncryptionEnforcementConfig(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        if (bucket.Encryption is null
            || (bucket.Encryption.CustomerManagedEncryptionEnforcementConfig is null
                && bucket.Encryption.CustomerSuppliedEncryptionEnforcementConfig is null
                && bucket.Encryption.GoogleManagedEncryptionEnforcementConfig is null))
        {
            Console.WriteLine($"No Encryption Enforcement Configuration found for bucket {bucketName}");
            return bucket;
        }

        bucket.Encryption = new Bucket.EncryptionData
        {
            DefaultKmsKeyName = bucket.Encryption.DefaultKmsKeyName
        };

        bucket = storage.UpdateBucket(bucket);
        Console.WriteLine($"The Encryption Enforcement Configuration has been removed from the bucket {bucketName}");
        return bucket;
    }
}
// [END storage_remove_all_encryption_enforcement_config]
