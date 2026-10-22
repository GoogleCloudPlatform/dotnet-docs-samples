/*
 * Copyright 2026 Google LLC
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

// [START secretmanager_update_secret_with_etag]

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UpdateSecretWithEtagSample
{
    // Synchronous version of the method
    public Secret UpdateSecretWithEtag(
        string projectId = "my-project",
        string secretId = "my-secret")
    {
        // Create the Secret Manager client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the secret.
        SecretName secretName = new SecretName(projectId, secretId);

        // Get the current secret to retrieve its ETag
        Secret currentSecret = client.GetSecret(secretName);
        string etag = currentSecret.Etag;

        // Build the updated secret with labels and ETag
        Secret secretToUpdate = new Secret
        {
            SecretName = secretName,
            Labels = { { "secretmanager", "rocks" } },
            Etag = etag
        };

        // Create a field mask for the fields to update
        FieldMask updateMask = new FieldMask { Paths = { "labels" } };

        // Update the secret
        Secret updatedSecret = client.UpdateSecret(
            new UpdateSecretRequest
            {
                Secret = secretToUpdate,
                UpdateMask = updateMask
            });

        // Print the updated secret name
        Console.WriteLine($"Updated secret: {updatedSecret.Name}");
        return updatedSecret;
    }
}
// [END secretmanager_update_secret_with_etag]
