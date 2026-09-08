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

// [START secretmanager_create_regional_secret_with_rotation]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateRegionalSecretWithRotationSample
{
    public Secret CreateRegionalSecretWithRotation(
        string projectId = "my-project",
        string secretId = "my-secret-with-rotation",
        string locationId = "us-central1",
        string topicName = "projects/my-project/topics/my-topic")
    {
        // Set rotation period to 24 hours
        int rotationPeriodHours = 24;

        // Set next rotation time to 24 hours from now
        DateTime nextRotationTime = DateTime.UtcNow.AddHours(24);

        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName location = new LocationName(projectId, locationId);

        // Convert DateTime to Timestamp for next rotation time
        Timestamp nextRotationTimestamp = Timestamp.FromDateTime(nextRotationTime.ToUniversalTime());

        // Convert rotation period to protobuf Duration
        Duration rotationPeriod = new Duration
        {
            Seconds = rotationPeriodHours * 3600 // Convert hours to seconds
        };

        // Build the secret with rotation configuration and topic
        Secret secret = new Secret
        {
            Topics = { new Topic { Name = topicName } },
            Rotation = new Rotation
            {
                NextRotationTime = nextRotationTimestamp,
                RotationPeriod = rotationPeriod
            }
        };

        // Call the API.
        Secret createdSecret = client.CreateSecret(location, secretId, secret);

        Console.WriteLine($"Created secret {createdSecret.SecretName} with rotation period {rotationPeriodHours} hours and topic {topicName}");

        return createdSecret;
    }
}
// [END secretmanager_create_regional_secret_with_rotation]
