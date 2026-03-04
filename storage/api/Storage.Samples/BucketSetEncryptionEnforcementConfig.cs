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

// [START storage_set_encryption_enforcement_config]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class BucketSetEncryptionEnforcementConfigSample
{
    /// <summary>
    /// Set the encryption enforcement configuration for a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="kmsKeyName">
    /// The full resource name of the Cloud KMS key (CMEK). 
    /// Required if <paramref name="enforceCmek"/> is true.
    /// </param>
    /// <param name="enforceCmek">If true, enforces Customer-Managed Encryption Key.</param>
    /// <param name="enforceGmek">If true, enforces Google-Managed Encryption Key.</param>
    /// <param name="restrictCsek">If true, restricts Customer-Supplied Encryption Key.</param>
    public Bucket SetBucketEncryptionEnforcementConfig(
        string bucketName = "your-unique-bucket-name",
        string kmsKeyName = null,
        bool enforceCmek = false,
        bool enforceGmek = false,
        bool restrictCsek = false)
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);

        if (bucket.Encryption == null)
        {
            bucket.Encryption = new Bucket.EncryptionData();
        }

        if (!string.IsNullOrEmpty(kmsKeyName))
        {
            bucket.Encryption.DefaultKmsKeyName = kmsKeyName;
            Console.WriteLine($"Default Key Set: {kmsKeyName}");
        }
        else
        {
            bucket.Encryption.DefaultKmsKeyName = null;
            Console.WriteLine("Default Key Set: None");
        }

        string cmek = enforceGmek ? "FullyRestricted" : "NotRestricted";
        string gmek = enforceCmek ? "FullyRestricted" : "NotRestricted";
        string csek = (enforceCmek || enforceGmek || restrictCsek) ? "FullyRestricted" : "NotRestricted";

        string message = enforceCmek ? "CMEK-only enforcement policy"
            : enforceGmek ? "GMEK-only enforcement policy"
            : restrictCsek ? "policy to restrict CSEK"
            : null;

        bucket.Encryption.CustomerManagedEncryptionEnforcementConfig = new Bucket.EncryptionData.CustomerManagedEncryptionEnforcementConfigData { RestrictionMode = cmek };
        bucket.Encryption.CustomerSuppliedEncryptionEnforcementConfig = new Bucket.EncryptionData.CustomerSuppliedEncryptionEnforcementConfigData { RestrictionMode = csek };
        bucket.Encryption.GoogleManagedEncryptionEnforcementConfig = new Bucket.EncryptionData.GoogleManagedEncryptionEnforcementConfigData { RestrictionMode = gmek };

        if (message != null)
        {
            Console.WriteLine($"Bucket {bucketName} updated with {message}");
        }

        var updatedBucket = storage.UpdateBucket(bucket);
        return updatedBucket;
    }
}
// [END storage_set_encryption_enforcement_config]
