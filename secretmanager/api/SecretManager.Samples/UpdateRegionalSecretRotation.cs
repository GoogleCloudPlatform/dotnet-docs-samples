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

// [START secretmanager_update_regional_secret_rotation_period]

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class UpdateRegionalSecretRotationSample
{
    public Secret UpdateRegionalSecretRotationPeriod(
        string projectId = "my-project",
        string secretId = "my-secret",
        string locationId = "us-central1")
    {
        // Build the secret name
        SecretName secretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId);

        // Set updated rotation period to 48 hours
        int newRotationPeriodHours = 48;

        // Create the Regional Secret Manager Client with the specific endpoint
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Create duration for the rotation period (48 hours in seconds)
        Duration rotationPeriod = new Duration
        {
            Seconds = newRotationPeriodHours * 3600 // Convert hours to seconds
        };

        // Create a field mask to update only the rotation_period field
        FieldMask updateMask = new FieldMask { Paths = { "rotation.rotation_period" } };

        // Update the secret with the new rotation period
        Secret secret = new Secret
        {
            SecretName = secretName,
            Rotation = new Rotation
            {
                RotationPeriod = rotationPeriod
            }
        };

        // Call the API
        Secret result = client.UpdateSecret(secret, updateMask);

        // Get the rotation period in hours for display
        double rotationHours = result.Rotation.RotationPeriod.Seconds / 3600.0;

        Console.WriteLine($"Updated secret {result.Name} rotation period to {rotationHours} hours");
        return result;
    }
}
// [END secretmanager_update_regional_secret_rotation_period]
