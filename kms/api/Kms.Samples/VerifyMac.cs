/*
 * Copyright 2021 Google LLC
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

// [START kms_verify_mac]

using Google.Cloud.Kms.V1;
using Google.Protobuf;

public class VerifyMacSample
{
    public bool VerifyMac(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      string data = "my data",
      byte[] signature = null)
    {
        // Build the key version name.
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId);

        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Convert the data and signatures into ByteStrings.
        ByteString dataByteString = ByteString.CopyFromUtf8(data);
        ByteString signatureByteString = ByteString.CopyFrom(signature);

        // Verify the signature.
        MacVerifyResponse result = client.MacVerify(keyVersionName, dataByteString, signatureByteString);

        // Return the result.
        return result.Success;
    }
}
// [END kms_verify_mac]
