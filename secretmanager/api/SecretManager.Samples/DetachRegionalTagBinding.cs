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

// [START secretmanager_detach_tag_from_regional_secret]

using Google.Cloud.ResourceManager.V3;
using Google.Cloud.SecretManager.V1;
using System;
using System.Linq;
using System.Threading.Tasks;

public class DetachTagFromRegionalSecretSample
{
    public async Task DetachTagFromRegionalSecretAsync(
        string projectId = "my-project",
        string locationId = "us-central1",
        string secretId = "my-secret",
        string tagValue = "tagValues/123456789012")
    {
        // Set up the endpoint for the regional resource manager
        string rmEndpoint = $"{locationId}-cloudresourcemanager.googleapis.com";

        // Create the Tag Bindings client with the regional endpoint
        TagBindingsClient tagBindingsClient = new TagBindingsClientBuilder
        {
            Endpoint = rmEndpoint
        }.Build();

        // Build the secret name
        string name = $"projects/{projectId}/locations/{locationId}/secrets/{secretId}";

        // Format the resource name for the secret
        string resourceName = $"//secretmanager.googleapis.com/{name}";

        // List all tag bindings for the secret to find the one to detach
        ListTagBindingsRequest listRequest = new ListTagBindingsRequest
        {
            Parent = resourceName
        };

        // Find the binding that matches the tag value
        TagBinding bindingToDelete = null;
        foreach (var binding in tagBindingsClient.ListTagBindings(listRequest))
        {
            if (binding.TagValue == tagValue)
            {
                bindingToDelete = binding;
                break;
            }
        }

        if (bindingToDelete == null)
        {
            Console.WriteLine($"No tag binding found for tag value {tagValue} on {name}");
            return;
        }

        // Create the delete request
        DeleteTagBindingRequest deleteRequest = new DeleteTagBindingRequest
        {
            Name = bindingToDelete.Name
        };

        // Delete the tag binding and wait for the operation to complete
        var operation = await tagBindingsClient.DeleteTagBindingAsync(deleteRequest);
        await operation.PollUntilCompletedAsync();

        Console.WriteLine($"Detached tag value {tagValue} from {name}");

    }
}
// [END secretmanager_detach_tag_from_regional_secret]
