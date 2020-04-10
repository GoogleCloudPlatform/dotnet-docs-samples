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

// [START kms_iam_add_member]
using System;

using Google.Cloud.Iam.V1;
using Google.Cloud.Kms.V1;

public class IamAddMemberSample
{

    public Policy IamAddMember(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key",
      string member = "user:foo@example.com")
    {
        // Create the client.
        var client = KeyManagementServiceClient.Create();

        // Construct the resource name.
        var resourceName = new CryptoKeyName(projectId, locationId, keyRingId, keyId);

        // The resource name could also be a Cloud KMS key ring.
        // var resourceName = new KeyRingName(projectId, locationId, keyRingId);

        // Build the request.
        var getRequest = new GetIamPolicyRequest
        {
            ResourceAsResourceName = resourceName,
        };

        // Get the current IAM policy.
        var policy = client.GetIamPolicy(getRequest);

        // Add the member to the policy.
        policy.AddRoleMember("roles/cloudkms.cryptoKeyEncrypterDecrypter", member);

        // Build the request to update the policy.
        var setRequest = new SetIamPolicyRequest
        {
            ResourceAsResourceName = resourceName,
            Policy = policy,
        };

        // Save the updated IAM policy.
        var result = client.SetIamPolicy(setRequest);

        // Return the resulting policy.
        return result;
    }
}
// [END kms_iam_add_member]
