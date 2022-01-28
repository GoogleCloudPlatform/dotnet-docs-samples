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

// [START kms_generate_random_bytes]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Kms.V1;

public class GenerateRandomBytesSample
{
    public byte[] GenerateRandomBytes(
      string projectId = "my-project", string locationId = "us-east1", int numBytes = 256)
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the location name.
        LocationName locationName = new LocationName(projectId, locationId);

        // Call the API.
        GenerateRandomBytesResponse result = client.GenerateRandomBytes(locationName.ToString(), numBytes, ProtectionLevel.Hsm);

        // The data comes back as raw bytes, which may include non-printable
        // characters. To print the result, you could encode it as base64.
        // string encodedData = result.Data.ToBase64();

        return result.Data.ToByteArray();
    }
}
// [END kms_generate_random_bytes]
