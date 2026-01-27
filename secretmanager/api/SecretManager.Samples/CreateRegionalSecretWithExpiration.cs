/*
 * Copyright 2025 Google LLC
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

// [START secretmanager_create_regional_secret_with_expire_time]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateRegionalSecretWithExpireTimeSample
{
    public Secret CreateRegionalSecretWithExpireTime(
        string projectId = "my-project",
        string secretId = "my-secret-with-expiry",
        string locationId = "us-central1")
    {
        // Set expiration time to 1 hour from now
        DateTime expireTime = DateTime.UtcNow.AddHours(1);

        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName location = new LocationName(projectId, locationId);

        // Convert DateTime to Timestamp
        Timestamp timestamp = Timestamp.FromDateTime(expireTime);

        // Build the secret with expiration time
        Secret secret = new Secret
        {
            ExpireTime = timestamp
        };

        // Call the API.
        Secret createdSecret = client.CreateSecret(location, secretId, secret);

        Console.WriteLine($"Created secret {createdSecret.SecretName} with expiration time {expireTime}");
        return createdSecret;
    }
}
// [END secretmanager_create_regional_secret_with_expire_time]
