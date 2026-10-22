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

// [START parametermanager_get_param_template]

using Google.Cloud.ParameterManager.V1;

public class GetParameterTemplateSample
{
    /// <summary>
    /// Gets a template.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="templateId">The ID of the template.</param>
    public Template GetParameterTemplate(
        string projectId,
        string templateId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the resource name.
        TemplateName templateName = new TemplateName(projectId, "global", templateId);

        // Call the API.
        Template template = client.GetTemplate(templateName);

        // Print the template name.
        Console.WriteLine($"Found template {template.Name} with format {template.Format}");

        return template;
    }
}
// [END parametermanager_get_param_template]
