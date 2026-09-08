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

// [START secretmanager_create_secret_with_rotation]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateSecretWithRotationSample
{
    public Secret CreateSecretWithRotation(
        string projectId = "my-project",
        string secretId = "my-rotating-secret",
        string topicName = "projects/my-project/topics/my-rotation-topic")
    {
        // Set rotation period to 24 hours
        int rotationPeriodHours = 24;

        // Calculate next rotation time (24 hours from now)
        DateTime nextRotationTime = DateTime.UtcNow.AddHours(24);

        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the parent resource name.
        ProjectName projectName = new ProjectName(projectId);

        // Convert rotation period to protobuf Duration
        Duration rotationPeriod = new Duration
        {
            Seconds = rotationPeriodHours * 3600 // Convert hours to seconds
        };

        // Convert DateTime to Timestamp for next rotation time
        Timestamp nextRotationTimestamp = Timestamp.FromDateTime(nextRotationTime.ToUniversalTime());

        // Build the secret with automatic replication and rotation configuration.
        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic(),
            },
            Topics = { new Topic { Name = topicName } },
            Rotation = new Rotation
            {
                NextRotationTime = nextRotationTimestamp,
                RotationPeriod = rotationPeriod
            }
        };

        // Call the API.
        Secret createdSecret = client.CreateSecret(projectName, secretId, secret);

        Console.WriteLine($"Created secret {createdSecret.SecretName} with rotation period {rotationPeriodHours} hours and topic {topicName}");

        return createdSecret;
    }
}
// [END secretmanager_create_secret_with_rotation]
