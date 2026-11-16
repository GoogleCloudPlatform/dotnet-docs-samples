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

// [START parametermanager_get_regional_param_tags]

using Google.Api.Gax;
using Google.Cloud.ParameterManager.V1;
using Google.Cloud.ResourceManager.V3;

public class GetRegionalParameterTagsSample
{
    /// <summary>
    /// Gets the tag bindings of a regional parameter.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="locationId">The ID of the region.</param>
    /// <param name="parameterId">The ID of the parameter.</param>
    public IEnumerable<TagBinding> GetRegionalParameterTags(
        string projectId,
        string locationId,
        string parameterId)
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

        // Get the parameter.
        ParameterName parameterName = new ParameterName(projectId, locationId, parameterId);
        Parameter parameter = client.GetParameter(parameterName);

        // Build the request.
        ListTagBindingsRequest request = new ListTagBindingsRequest
        {
            Parent = $"//parametermanager.googleapis.com/{parameter.Name}"
        };

        // Call the API.
        PagedEnumerable<ListTagBindingsResponse, TagBinding> response = tagBindingsClient.ListTagBindings(request);

        // Print each tag value.
        foreach (TagBinding tagBinding in response)
        {
            Console.WriteLine($"Found regional parameter tag: {tagBinding.TagValue}");
        }

        return response;
    }
}
// [END parametermanager_get_regional_param_tags]
