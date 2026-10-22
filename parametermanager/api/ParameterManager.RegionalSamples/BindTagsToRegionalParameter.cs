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

// [START parametermanager_bind_tags_to_regional_param]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;
using Google.Cloud.ResourceManager.V3;

public class BindTagsToRegionalParameterSample
{
    /// <summary>
    /// Creates a regional parameter and binds a tag value to it.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="locationId">The ID of the region.</param>
    /// <param name="parameterId">The ID of the parameter.</param>
    /// <param name="tagValue">The tag value resource name.</param>
    public TagBinding BindTagsToRegionalParameter(
        string projectId,
        string locationId,
        string parameterId,
        string tagValue)
    {
        // Define the regional endpoint.
        string regionalEndpoint = $"parametermanager.{locationId}.rep.googleapis.com";

        // Create the client with the regional endpoint.
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        // Create the tag bindings client with the regional endpoint.
        TagBindingsClient tagBindingsClient = new TagBindingsClientBuilder
        {
            Endpoint = $"{locationId}-cloudresourcemanager.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName parent = new LocationName(projectId, locationId);

        // Create the parameter.
        Parameter parameter = client.CreateParameter(parent, new Parameter(), parameterId);

        // Print the created parameter name.
        Console.WriteLine($"Created regional parameter: {parameter.Name}");

        // Build the tag binding.
        TagBinding tagBinding = new TagBinding
        {
            Parent = $"//parametermanager.googleapis.com/{parameter.Name}",
            TagValue = tagValue
        };

        // Call the API and wait for the operation to complete.
        TagBinding createdTagBinding = tagBindingsClient.CreateTagBinding(tagBinding).PollUntilCompleted().Result;

        // Print the created tag binding name.
        Console.WriteLine($"Created tag binding: {createdTagBinding.Name}");

        return createdTagBinding;
    }
}
// [END parametermanager_bind_tags_to_regional_param]
