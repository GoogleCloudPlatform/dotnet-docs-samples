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

// [START parametermanager_list_param_template_versions]

using Google.Api.Gax;
using Google.Cloud.ParameterManager.V1;

public class ListParameterTemplateVersionsSample
{
    /// <summary>
    /// Lists the versions of a template.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="templateId">The ID of the template.</param>
    public IEnumerable<TemplateVersion> ListParameterTemplateVersions(
        string projectId,
        string templateId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name.
        TemplateName parent = new TemplateName(projectId, "global", templateId);

        // Call the API.
        PagedEnumerable<ListTemplateVersionsResponse, TemplateVersion> response = client.ListTemplateVersions(parent);

        // Print each template version name.
        foreach (TemplateVersion templateVersion in response)
        {
            Console.WriteLine($"Found template version: {templateVersion.Name}");
        }

        return response;
    }
}
// [END parametermanager_list_param_template_versions]
