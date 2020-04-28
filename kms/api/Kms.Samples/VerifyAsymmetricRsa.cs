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

// [START kms_verify_asymmetric_signature_rsa]

using Google.Cloud.Kms.V1;
using System;
using System.Security.Cryptography;
using System.Text;

public class VerifyAsymmetricSignatureRsaSample
{
    public bool VerifyAsymmetricSignatureRsa(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      string message = "my message",
      byte[] signature = null)
    {
        // Build the key version name.
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId);

        // Calculate the digest of the message.
        SHA256 sha256 = SHA256.Create();
        byte[] digest = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));

        // Get the public key.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        PublicKey publicKey = client.GetPublicKey(keyVersionName);

        // Split the key into blocks and base64-decode the PEM parts.
        string[] blocks = publicKey.Pem.Split("-", StringSplitOptions.RemoveEmptyEntries);
        byte[] pem = Convert.FromBase64String(blocks[1]);

        // Create a new RSA key.
        RSA rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(pem, out _);

        // Verify the signature.
        bool verified = rsa.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        // Return the result.
        return verified;
    }
}
// [END kms_verify_asymmetric_signature_rsa]
