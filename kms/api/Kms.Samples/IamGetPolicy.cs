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

// [START kms_iam_get_policy]

using Google.Cloud.Iam.V1;
using Google.Cloud.Kms.V1;
using System;

public class IamGetPolicySample
{
    public Policy IamGetPolicy(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the resource name.
        CryptoKeyName resourceName = new CryptoKeyName(projectId, locationId, keyRingId, keyId);

        // The resource name could also be a key ring.
        // var resourceName = new KeyRingName(projectId, locationId, keyRingId);

        // Get the current IAM policy.
        Policy policy = client.GetIamPolicy(resourceName);

        // Print the policy.
        foreach (Binding b in policy.Bindings)
        {
            String role = b.Role;

            foreach (String member in b.Members)
            {
                // ...
            }
        }

        // Return the policy.
        return policy;
    }
}
// [END kms_iam_get_policy]
