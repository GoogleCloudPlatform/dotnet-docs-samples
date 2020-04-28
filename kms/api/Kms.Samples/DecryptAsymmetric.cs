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

// [START kms_decrypt_asymmetric]

using Google.Cloud.Kms.V1;
using Google.Protobuf;
using System.Text;

public class DecryptAsymmetricSample
{
    public string DecryptAsymmetric(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      byte[] ciphertext = null)
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the key version name.
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId);

        // Call the API.
        AsymmetricDecryptResponse result = client.AsymmetricDecrypt(keyVersionName, ByteString.CopyFrom(ciphertext));

        // Get the plaintext. Cryptographic plaintexts and ciphertexts are
        // always byte arrays.
        byte[] plaintext = result.Plaintext.ToByteArray();

        // Return the result.
        return Encoding.UTF8.GetString(plaintext);
    }
}
// [END kms_decrypt_asymmetric]
