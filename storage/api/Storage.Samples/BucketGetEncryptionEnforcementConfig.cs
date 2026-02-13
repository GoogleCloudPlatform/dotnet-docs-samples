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

// [START storage_get_encryption_enforcement_config]

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;

public class BucketGetEncryptionEnforcementConfigSample
{
    /// <summary>
    /// Get the encryption enforcement configuration for the bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    public Bucket.EncryptionData BucketGetEncryptionEnforcementConfig(string bucketName = "your-unique-bucket-name")
    {
        var storage = StorageClient.Create();
        var bucket = storage.GetBucket(bucketName);
        Console.WriteLine($"Encryption Enforcement Configuration for bucket {bucketName} is as follows:");

        if (bucket.Encryption == null)
        {
            Console.WriteLine("No Encryption Configuration is found (Default GMEK is active)");
            return bucket.Encryption;
        }

        var gmConfig = bucket.Encryption.GoogleManagedEncryptionEnforcementConfig;
        if (gmConfig != null)
        {
            Console.WriteLine($"Google Managed (GMEK) Enforcement Restriction Mode: {gmConfig.RestrictionMode}");
            Console.WriteLine($"Google Managed (GMEK) Enforcement Effective Time: {gmConfig.EffectiveTimeRaw}");
        }
        var cmConfig = bucket.Encryption.CustomerManagedEncryptionEnforcementConfig;
        if (cmConfig != null)
        {
            Console.WriteLine($"Customer Managed (CMEK) Enforcement Restriction Mode: {cmConfig.RestrictionMode}");
            Console.WriteLine($"Customer Managed (CMEK) Enforcement Effective Time: {cmConfig.EffectiveTimeRaw}");
        }
        var csConfig = bucket.Encryption.CustomerSuppliedEncryptionEnforcementConfig;
        if (csConfig != null)
        {
            Console.WriteLine($"Customer Supplied (CSEK) Enforcement Restriction Mode: {csConfig.RestrictionMode}");
            Console.WriteLine($"Customer Supplied (CSEK) Enforcement Effective Time: {csConfig.EffectiveTimeRaw}");
        }
        return bucket.Encryption;
    }
}
// [END storage_get_encryption_enforcement_config]
