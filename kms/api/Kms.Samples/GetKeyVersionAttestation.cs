/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START kms_get_key_version_attestation]

using Google.Cloud.Kms.V1;
using System;

public class GetKeyVersionAttestationSample
{
    public byte[] GetKeyVersionAttestation(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the key name.
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId);

        // Call the API.
        CryptoKeyVersion result = client.GetCryptoKeyVersion(keyVersionName);

        // Only HSM keys have an attestation. For other key types, the attestion
        // will be nil.
        KeyOperationAttestation attestation = result.Attestation;
        if (attestation == null)
        {
            throw new InvalidOperationException("no attestation");
        }

        // Return the attestation.
        return attestation.Content.ToByteArray();
    }
}
// [END kms_get_key_version_attestation]
