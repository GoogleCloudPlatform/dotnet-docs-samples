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

// [START secretmanager_delete_regional_secret_rotation]

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class DeleteRegionalSecretRotationSample
{
    public Secret DeleteRegionalSecretRotation(
        string projectId = "my-project",
        string secretId = "my-secret-with-rotation",
        string locationId = "us-central1")
    {
        // Construct the secret name from the component parts
        string secretName = $"projects/{projectId}/locations/{locationId}/secrets/{secretId}";

        // Create the Regional Secret Manager Client
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Create a field mask to update only the rotation field
        FieldMask updateMask = new FieldMask { Paths = { "rotation" } };

        // Create the secret object with just the name
        Secret secret = new Secret
        {
            Name = secretName
        };

        // Update the secret to remove the rotation configuration
        Secret result = client.UpdateSecret(new UpdateSecretRequest
        {
            Secret = secret,
            UpdateMask = updateMask
        });

        Console.WriteLine($"Removed rotation from secret {result.Name}");
        return result;
    }
}
// [END secretmanager_delete_regional_secret_rotation]
