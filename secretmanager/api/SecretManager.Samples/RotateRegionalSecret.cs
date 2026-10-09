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

// [START secretmanager_rotate_regional_secret]

using Google.Cloud.SecretManager.V1;
using System;

public class RotateRegionalSecretSample
{
    /// <summary>
    /// Triggers an adhoc rotation for the managed CLOUD_SQL_DB_CREDENTIALS typed
    /// secret.
    /// </summary>
    public SecretVersion RotateRegionalSecret(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret"
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the request.
        RotateSecretRequest request = new RotateSecretRequest
        {
            ParentAsSecretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId),
        };

        // Call the API.
        SecretVersion version = client.RotateSecret(request);
        Console.WriteLine($"Rotated secret, created secret version: {version.Name}");
        return version;
    }
}
// [END secretmanager_rotate_regional_secret]
