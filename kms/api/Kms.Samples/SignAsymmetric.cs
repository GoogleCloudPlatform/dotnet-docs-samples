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

// [START kms_sign_asymmetric]

using Google.Cloud.Kms.V1;
using Google.Protobuf;
using System.Security.Cryptography;
using System.Text;

public class SignAsymmetricSample
{
    public byte[] SignAsymmetric(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      string message = "Sample message")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the key version name.
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId);

        // Convert the message into bytes. Cryptographic plaintexts and
        // ciphertexts are always byte arrays.
        byte[] plaintext = Encoding.UTF8.GetBytes(message);

        // Calculate the digest.
        SHA256 sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(plaintext);

        // Build the digest.
        //
        // Note: Key algorithms will require a varying hash function. For
        // example, EC_SIGN_P384_SHA384 requires SHA-384.
        Digest digest = new Digest
        {
            Sha256 = ByteString.CopyFrom(hash),
        };

        // Call the API.
        AsymmetricSignResponse result = client.AsymmetricSign(keyVersionName, digest);

        // Get the signature.
        byte[] signature = result.Signature.ToByteArray();

        // Return the result.
        return signature;
    }
}
// [END kms_sign_asymmetric]
