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

// [START secretmanager_bind_tags_to_secret]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ResourceManager.V3;
using Google.Cloud.SecretManager.V1;
using System;
using System.Threading.Tasks;

public class BindTagsToSecretSample
{
    public async Task BindTagsToSecretAsync(
        string projectId = "my-project",
        string secretId = "my-secret",
        string tagValue = "tagValues/123456789012")
    {
        // Create the Secret Manager client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the parent resource name.
        ProjectName projectName = new ProjectName(projectId);

        // Build the secret.
        Secret secret = new Secret
        {
            Replication = new Replication
            {
                Automatic = new Replication.Types.Automatic(),
            },
        };

        // Call the API.
        Secret createdSecret = client.CreateSecret(projectName, secretId, secret);

        // Print the new secret name.
        Console.WriteLine($"Created secret: {createdSecret.Name}");

        // Create the resource manager client.
        TagBindingsClient resourceManagerClient = TagBindingsClient.Create();

        // Format the resource name for the secret.
        string resourceName = $"//secretmanager.googleapis.com/{createdSecret.Name}";

        // Create the tag binding.
        TagBinding tagBinding = new TagBinding
        {
            Parent = resourceName,
            TagValue = tagValue
        };

        // Create the tag binding and wait for the operation to complete.
        var operation = await resourceManagerClient.CreateTagBindingAsync(tagBinding);
        await operation.PollUntilCompletedAsync();

        Console.WriteLine($"Created Tag Binding: {secret.Name}");

    }
}
// [END secretmanager_bind_tags_to_secret]
