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

// [START kms_sign_mac]

using Google.Cloud.Kms.V1;
using Google.Protobuf;

public class SignMacSample
{
    public byte[] SignMac(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      string data = "Sample data")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the key version name.
        CryptoKeyVersionName keyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId);

        // Convert the data into a ByteString.
        ByteString dataByteString = ByteString.CopyFromUtf8(data);

        // Call the API.
        MacSignResponse result = client.MacSign(keyVersionName, dataByteString);

        // The data comes back as raw bytes, which may include non-printable
        // characters. To print the result, you could encode it as base64.
        // string encodedSignature = result.Mac.ToBase64();

        // Get the signature.
        byte[] signature = result.Mac.ToByteArray();

        // Return the result.
        return signature;
    }
}
// [END kms_sign_mac]
