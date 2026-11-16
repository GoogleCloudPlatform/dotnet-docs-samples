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

// [START parametermanager_create_param_with_tags]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;

public class CreateParameterWithTagsSample
{
    /// <summary>
    /// Creates a parameter with a tag.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="parameterId">The ID of the parameter.</param>
    /// <param name="tagKey">The tag key resource name.</param>
    /// <param name="tagValue">The tag value resource name.</param>
    public Parameter CreateParameterWithTags(
        string projectId,
        string parameterId,
        string tagKey,
        string tagValue)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name.
        LocationName parent = new LocationName(projectId, "global");

        // Build the parameter with the tag.
        Parameter parameter = new Parameter
        {
            Tags =
            {
                { tagKey, tagValue }
            }
        };

        // Call the API.
        Parameter createdParameter = client.CreateParameter(parent, parameter, parameterId);

        // Print the created parameter name.
        Console.WriteLine($"Created parameter {createdParameter.Name} with tag {tagKey}: {tagValue}");

        return createdParameter;
    }
}
// [END parametermanager_create_param_with_tags]
