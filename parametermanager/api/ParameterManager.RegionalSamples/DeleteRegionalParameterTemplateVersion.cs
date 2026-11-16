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

// [START parametermanager_delete_regional_param_template_version]

using Google.Cloud.ParameterManager.V1;

public class DeleteRegionalParameterTemplateVersionSample
{
    /// <summary>
    /// Deletes a regional template version.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="locationId">The ID of the region.</param>
    /// <param name="templateId">The ID of the template.</param>
    /// <param name="versionId">The ID of the template version.</param>
    public void DeleteRegionalParameterTemplateVersion(
        string projectId,
        string locationId,
        string templateId,
        string versionId)
    {
        // Define the regional endpoint.
        string regionalEndpoint = $"parametermanager.{locationId}.rep.googleapis.com";

        // Create the client with the regional endpoint.
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        // Build the resource name.
        TemplateVersionName templateVersionName = new TemplateVersionName(projectId, locationId, templateId, versionId);

        // Call the API.
        client.DeleteTemplateVersion(templateVersionName);

        // Print a confirmation message.
        Console.WriteLine($"Deleted regional template version: {templateVersionName}");
    }
}
// [END parametermanager_delete_regional_param_template_version]
