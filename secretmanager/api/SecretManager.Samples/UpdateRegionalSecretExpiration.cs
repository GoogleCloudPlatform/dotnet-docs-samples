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

// [START secretmanager_update_regional_secret_expiration]

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class UpdateRegionalSecretExpirationSample
{
    public void UpdateRegionalSecretExpiration(
        string projectId = "my-project",
        string secretId = "my-secret",
        string locationId = "us-central1")
    {
        // Set new expiration time to 2 hours from now
        DateTime newExpireTime = DateTime.UtcNow.AddHours(2);

        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the secret name
        SecretName secretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId);

        // Convert DateTime to Timestamp
        Timestamp timestamp = Timestamp.FromDateTime(newExpireTime);

        // Build the secret with the new expiration time
        Secret secret = new Secret
        {
            SecretName = secretName,
            ExpireTime = timestamp
        };

        // Build the field mask for the fields to update
        FieldMask updateMask = new FieldMask { Paths = { "expire_time" } };

        // Update the secret
        Secret updatedSecret = client.UpdateSecret(secret, updateMask);

        Console.WriteLine($"Updated secret {updatedSecret.SecretName} expiration time to {newExpireTime}");
    }
}
// [END secretmanager_update_regional_secret_expiration]
