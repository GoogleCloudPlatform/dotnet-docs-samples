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

public class DecryptAsymmetricSample
{
    public string DecryptAsymmetric(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      byte[] ciphertext = null)
    {
        // Create the client.
        var client = KeyManagementServiceClient.Create();

        // Build the request.
        var request = new AsymmetricDecryptRequest
        {
            CryptoKeyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId),
            Ciphertext = ByteString.CopyFrom(ciphertext),
        };

        // Call the API.
        var result = client.AsymmetricDecrypt(request);

        // Return the result.
        return result.Plaintext.ToStringUtf8();
    }
}
// [END kms_decrypt_asymmetric]
