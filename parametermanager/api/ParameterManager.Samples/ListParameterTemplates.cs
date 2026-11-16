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

// [START parametermanager_list_param_templates]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;

public class ListParameterTemplatesSample
{
    /// <summary>
    /// Lists templates.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    public IEnumerable<Template> ListParameterTemplates(
        string projectId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name.
        LocationName parent = new LocationName(projectId, "global");

        // Call the API.
        PagedEnumerable<ListTemplatesResponse, Template> response = client.ListTemplates(parent);

        // Print each template name.
        foreach (Template template in response)
        {
            Console.WriteLine($"Found template {template.Name} with format {template.Format}");
        }

        return response;
    }
}
// [END parametermanager_list_param_templates]
