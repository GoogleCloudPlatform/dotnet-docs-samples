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

// [START secretmanager_create_regional_secret_with_topic]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using System;

public class CreateRegionalSecretWithTopicSample
{
    public Secret CreateRegionalSecretWithTopic(
        string projectId = "my-project",
        string secretId = "my-secret-with-topic",
        string locationId = "us-central1",
        string topicName = "projects/my-project/topics/my-topic")
    {
        // Create the Regional Secret Manager Client
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the resource name of the parent project with location
        LocationName parent = new LocationName(projectId, locationId);

        // Create the secret with a topic for notifications
        Secret secretToCreate = new Secret
        {
            Topics = { new Topic { Name = topicName } }
        };

        // Create the secret
        Secret createdSecret = client.CreateSecret(parent, secretId, secretToCreate);

        Console.WriteLine($"Created secret {createdSecret.Name} with topic {topicName}");
        return createdSecret;
    }
}
// [END secretmanager_create_regional_secret_with_topic]
