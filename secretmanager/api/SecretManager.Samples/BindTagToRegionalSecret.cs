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

// [START secretmanager_bind_tags_to_regional_secret]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ResourceManager.V3;
using Google.Cloud.SecretManager.V1;
using System;
using System.Threading.Tasks;

public class BindTagsToRegionalSecretSample
{

    public async Task<TagBinding> BindTagsToRegionalSecretAsync(
        string projectId = "my-project",
        string locationId = "us-central1",
        string secretId = "my-secret",
        string tagValue = "tagValues/123456789012")
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName location = new LocationName(projectId, locationId);

        // Build the secret.
        Secret secret = new Secret();

        // Call the API to create the secret.
        Secret createdSecret = client.CreateSecret(location, secretId, secret);

        // Print the new secret name.
        Console.WriteLine($"Created regional secret: {createdSecret.Name}");
        string resourceManagerApiEndpoint = $"{locationId}-cloudresourcemanager.googleapis.com";

        // Create the resource manager client
        TagBindingsClientBuilder tagBindingsClientBuilder = new TagBindingsClientBuilder
        {
            Endpoint = resourceManagerApiEndpoint
        };

        TagBindingsClient resourceManagerClient = tagBindingsClientBuilder.Build();

        // Create the tag binding
        CreateTagBindingRequest request = new CreateTagBindingRequest
        {
            TagBinding = new TagBinding
            {
                Parent = $"//secretmanager.googleapis.com/{createdSecret.Name}",
                TagValue = tagValue
            }
        };

        // Create the tag binding
        var operation = await resourceManagerClient.CreateTagBindingAsync(request);

        // Wait for the operation to complete
        await operation.PollUntilCompletedAsync();
        TagBinding tagBinding = operation.Result;

        // Print the tag binding.
        Console.WriteLine($"Created tag binding: {createdSecret.Name}");
        return tagBinding;
    }
}
// [END secretmanager_bind_tags_to_regional_secret]
