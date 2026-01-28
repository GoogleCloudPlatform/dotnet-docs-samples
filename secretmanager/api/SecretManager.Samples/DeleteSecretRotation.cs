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

// [START secretmanager_delete_secret_rotation]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class DeleteSecretRotationSample
{
    public Secret DeleteSecretRotation(string projectId = "my-project", string secretId = "my-secret-with-rotation")
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the secret.
        SecretName secretName = new SecretName(projectId, secretId);

        // Create the update mask to remove the rotation field.
        FieldMask updateMask = new FieldMask { Paths = { "rotation" } };

        // Build the secret with empty fields (will be removed by the update mask).
        Secret secretToUpdate = new Secret
        {
            SecretName = secretName
        };

        // Call the API.
        Secret updatedSecret = client.UpdateSecret(new UpdateSecretRequest
        {
            Secret = secretToUpdate,
            UpdateMask = updateMask
        });

        Console.WriteLine($"Removed rotation from secret {updatedSecret.SecretName}");

        return updatedSecret;
    }
}
// [END secretmanager_delete_secret_rotation]
