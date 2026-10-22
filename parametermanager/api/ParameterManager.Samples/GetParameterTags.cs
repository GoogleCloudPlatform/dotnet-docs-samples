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

// [START parametermanager_get_param_tags]

using Google.Api.Gax;
using Google.Cloud.ParameterManager.V1;
using Google.Cloud.ResourceManager.V3;

public class GetParameterTagsSample
{
    /// <summary>
    /// Gets the tag bindings of a parameter.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="parameterId">The ID of the parameter.</param>
    public IEnumerable<TagBinding> GetParameterTags(
        string projectId,
        string parameterId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Create the tag bindings client.
        TagBindingsClient tagBindingsClient = TagBindingsClient.Create();

        // Get the parameter.
        ParameterName parameterName = new ParameterName(projectId, "global", parameterId);
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
            Console.WriteLine($"Found parameter tag: {tagBinding.TagValue}");
        }

        return response;
    }
}
// [END parametermanager_get_param_tags]
