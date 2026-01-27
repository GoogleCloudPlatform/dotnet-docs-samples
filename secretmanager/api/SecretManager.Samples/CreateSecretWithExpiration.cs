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

// [START secretmanager_create_secret_with_expiration]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateSecretWithExpirationSample
{
    public Secret CreateSecretWithExpiration(string projectId = "my-project", string secretId = "my-secret-with-expiry")
    {
        // Calculate expiration time (1 hour from now)
        DateTime expireTime = DateTime.UtcNow.AddHours(1);

        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the parent resource name.
        ProjectName projectName = new ProjectName(projectId);

        // Convert DateTime to Timestamp
        Timestamp timestamp = Timestamp.FromDateTime(expireTime.ToUniversalTime());

        // Build the secret with automatic replication and expiration time.
        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic(),
            },
            ExpireTime = timestamp
        };

        // Call the API.
        Secret createdSecret = client.CreateSecret(projectName, secretId, secret);

        Console.WriteLine($"Created secret {createdSecret.SecretName} with expiration time {expireTime}");
        return createdSecret;
    }
}
// [END secretmanager_create_secret_with_expiration]
