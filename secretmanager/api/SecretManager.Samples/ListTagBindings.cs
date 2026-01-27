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

// [START secretmanager_list_tag_bindings]

using Google.Cloud.ResourceManager.V3;
using Google.Cloud.SecretManager.V1;
using System;

public class ListTagBindingsSample
{
    public void ListTagBindings(
        string projectId = "my-project",
        string secretId = "my-secret")
    {
        // Create the Resource Manager client.
        TagBindingsClient client = TagBindingsClient.Create();

        // Build the resource name of the parent secret.
        SecretName secretName = new SecretName(projectId, secretId);

        // Format the parent resource for tag bindings.
        string parent = $"//secretmanager.googleapis.com/{secretName}";

        // Create the request.
        ListTagBindingsRequest request = new ListTagBindingsRequest
        {
            Parent = parent
        };

        // List all tag bindings.
        var bindings = client.ListTagBindings(request);
        bool foundBindings = false;

        Console.WriteLine($"Tag bindings for {secretName}:");
        foreach (var binding in bindings)
        {
            Console.WriteLine($"- Tag Value: {binding.TagValue}");
            foundBindings = true;
        }

        if (!foundBindings)
        {
            Console.WriteLine($"No tag bindings found for {secretName}.");
        }
    }
}
// [END secretmanager_list_tag_bindings]
