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
using System.Security.Cryptography;
using System.Text;

using Google.Cloud.Kms.V1;
using Google.Protobuf;

public class SignAsymmetricSample
{

    public byte[] SignAsymmetric(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      string message = "Sample message")
    {
        // Create the client.
        var client = KeyManagementServiceClient.Create();

        // Calculate the message digest.
        var sha256 = SHA256.Create();
        var digest = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));

        // Build the request.
        //
        // Note: Key algorithms will require a varying hash function. For
        // example, EC_SIGN_P384_SHA384 requires SHA-384.
        var request = new AsymmetricSignRequest
        {
            CryptoKeyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId),
            Digest = new Digest
            {
                Sha256 = ByteString.CopyFrom(digest),
            },
        };

        // Call the API.
        var result = client.AsymmetricSign(request);

        // Return the result.
        return result.Signature.ToByteArray();
    }
}
// [END kms_sign_asymmetric]
