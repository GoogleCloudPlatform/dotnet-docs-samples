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

// [START kms_update_key_update_labels]


using Google.Cloud.Kms.V1;
using Google.Protobuf.WellKnownTypes;

public class UpdateKeyUpdateLabelsSample
{
    public CryptoKey UpdateKeyUpdateLabels(string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the key name.
        CryptoKeyName keyName = new CryptoKeyName(projectId, locationId, keyRingId, keyId);

        //
        // Step 1 - get the current set of labels on the key
        //

        // Get the current key.
        CryptoKey key = client.GetCryptoKey(keyName);


        //
        // Step 2 - add a label to the list of labels
        //

        // Add a new label
        key.Labels["new_label"] = "new_value";

        // Build the update mask.
        FieldMask fieldMask = new FieldMask
        {
            Paths = { "labels" }
        };

        // Call the API.
        CryptoKey result = client.UpdateCryptoKey(key, fieldMask);

        // Return the updated key.
        return result;
    }
}
// [END kms_update_key_update_labels]
