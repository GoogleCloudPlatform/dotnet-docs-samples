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

// [START secretmanager_list_regional_secret_tag_bindings]

using Google.Cloud.ResourceManager.V3;
using Google.Cloud.SecretManager.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ListRegionalSecretTagBindingsSample
{
    public List<TagBinding> ListRegionalSecretTagBindings(
        string projectId = "my-project",
        string locationId = "us-central1",
        string secretId = "my-regional-secret")
    {
        // Set up the endpoint for the regional resource manager
        string rmEndpoint = $"{locationId}-cloudresourcemanager.googleapis.com";

        // Create the Tag Bindings client with the regional endpoint
        TagBindingsClient tagBindingsClient = new TagBindingsClientBuilder
        {
            Endpoint = rmEndpoint
        }.Build();

        string name = $"projects/{projectId}/locations/{locationId}/secrets/{secretId}";

        // Format the parent resource for the tag bindings request
        string parent = $"//secretmanager.googleapis.com/{name}";

        // List the tag bindings
        Console.WriteLine($"Tag bindings for {name}:");
        bool foundBindings = false;

        // Use the ListTagBindings method to get all tag bindings
        ListTagBindingsRequest request = new ListTagBindingsRequest
        {
            Parent = parent
        };

        List<TagBinding> tagBindings = new List<TagBinding>();
        // Iterate through the results
        foreach (var binding in tagBindingsClient.ListTagBindings(request))
        {
            Console.WriteLine($"- Tag Value: {binding.TagValue}");
            foundBindings = true;
            tagBindings.Add(binding);
        }

        if (!foundBindings)
        {
            Console.WriteLine($"No tag bindings found for {name}.");
        }
        return tagBindings;
    }
}
// [END secretmanager_list_regional_secret_tag_bindings]
