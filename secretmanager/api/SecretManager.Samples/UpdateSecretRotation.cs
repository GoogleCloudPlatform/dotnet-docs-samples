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

// [START secretmanager_update_secret_rotation]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class UpdateSecretRotationSample
{
    public Secret UpdateSecretRotation(string projectId = "my-project", string secretId = "my-secret-with-rotation")
    {
        int newRotationPeriodHours = 48;

        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the secret.
        SecretName secretName = new SecretName(projectId, secretId);

        // Convert rotation period to protobuf Duration
        Duration rotationPeriod = new Duration
        {
            Seconds = newRotationPeriodHours * 3600 // Convert hours to seconds
        };

        // Create the update mask
        FieldMask updateMask = new FieldMask { Paths = { "rotation.rotation_period" } };

        // Build the secret with updated rotation period
        Secret secretToUpdate = new Secret
        {
            SecretName = secretName,
            Rotation = new Rotation
            {
                RotationPeriod = rotationPeriod
            }
        };

        // Call the API.
        Secret updatedSecret = client.UpdateSecret(new UpdateSecretRequest
        {
            Secret = secretToUpdate,
            UpdateMask = updateMask
        });

        double rotationHours = updatedSecret.Rotation.RotationPeriod.Seconds / 3600.0;
        Console.WriteLine($"Updated secret {updatedSecret.SecretName} rotation period to {rotationHours} hours");

        return updatedSecret;
    }
}
// [END secretmanager_update_secret_rotation]
