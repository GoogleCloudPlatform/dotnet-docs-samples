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

// [START secretmanager_detach_tag]

using Google.Cloud.ResourceManager.V3;
using Google.Cloud.SecretManager.V1;
using System;
using System.Threading.Tasks;

public class DetachTagSample
{
    public async Task DetachTagAsync(
        string projectId = "my-project",
        string secretId = "my-secret",
        string tagValue = "tagValues/123456789012")
    {
        // Create the Resource Manager client.
        TagBindingsClient rmClient = TagBindingsClient.Create();

        // Create the Secret Manager client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the resource name of the parent secret.
        SecretName secretName = new SecretName(projectId, secretId);

        // Format the parent resource for tag bindings.
        string parent = $"//secretmanager.googleapis.com/{secretName}";

        // Find the binding name for the given tag value
        string bindingName = null;
        ListTagBindingsRequest listRequest = new ListTagBindingsRequest
        {
            Parent = parent
        };

        // Search for the binding with the specified tag value
        var bindings = rmClient.ListTagBindings(listRequest);
        foreach (var binding in bindings)
        {
            if (binding.TagValue == tagValue)
            {
                bindingName = binding.Name;
                break;
            }
        }

        if (bindingName == null)
        {
            Console.WriteLine($"Tag binding for value {tagValue} not found on {secretName}.");
            return;
        }

        // Delete the tag binding
        DeleteTagBindingRequest deleteRequest = new DeleteTagBindingRequest
        {
            Name = bindingName
        };

        // Delete the binding and wait for the operation to complete
        var operation = await rmClient.DeleteTagBindingAsync(deleteRequest);
        await operation.PollUntilCompletedAsync();

        Console.WriteLine($"Detached tag value {tagValue} from {secretName}");
    }
}
// [END secretmanager_detach_tag]
