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

// [START kms_create_key_ring]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Kms.V1;

public class CreateKeyRingSample
{
    public KeyRing CreateKeyRing(
      string projectId = "my-project", string locationId = "us-east1",
      string id = "my-key-ring")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the parent location name.
        LocationName locationName = new LocationName(projectId, locationId);

        // Build the key ring.
        KeyRing keyRing = new KeyRing { };

        // Call the API.
        KeyRing result = client.CreateKeyRing(locationName, id, keyRing);

        // Return the result.
        return result;
    }
}
// [END kms_create_key_ring]
