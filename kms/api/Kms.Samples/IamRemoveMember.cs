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

// [START kms_iam_remove_member]

using Google.Cloud.Iam.V1;
using Google.Cloud.Kms.V1;

public class IamRemoveMemberSample
{
    public Policy IamRemoveMember(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key",
      string member = "user:foo@example.com")
    {
        // Create the client.
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        // Build the resource name.
        CryptoKeyName resourceName = new CryptoKeyName(projectId, locationId, keyRingId, keyId);

        // The resource name could also be a key ring.
        // var resourceName = new KeyRingName(projectId, locationId, keyRingId);

        // Get the current IAM policy.
        Policy policy = client.GetIamPolicy(resourceName);

        // Add the member to the policy.
        policy.RemoveRoleMember("roles/cloudkms.cryptoKeyEncrypterDecrypter", member);

        // Save the updated IAM policy.
        Policy result = client.SetIamPolicy(resourceName, policy);

        // Return the resulting policy.
        return result;
    }
}
// [END kms_iam_remove_member]
