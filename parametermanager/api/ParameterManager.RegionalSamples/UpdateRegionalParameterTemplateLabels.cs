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

// [START parametermanager_update_regional_param_template_labels]

using Google.Cloud.ParameterManager.V1;
using Google.Protobuf.WellKnownTypes;

public class UpdateRegionalParameterTemplateLabelsSample
{
    /// <summary>
    /// Updates the labels of a regional template.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="locationId">The ID of the region.</param>
    /// <param name="templateId">The ID of the template.</param>
    /// <param name="key">The label key.</param>
    /// <param name="value">The label value.</param>
    public Template UpdateRegionalParameterTemplateLabels(
        string projectId,
        string locationId,
        string templateId,
        string key,
        string value)
    {
        // Define the regional endpoint.
        string regionalEndpoint = $"parametermanager.{locationId}.rep.googleapis.com";

        // Create the client with the regional endpoint.
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        // Build the resource name.
        TemplateName templateName = new TemplateName(projectId, locationId, templateId);

        // Get the template and set the label.
        Template template = client.GetTemplate(templateName);
        template.Labels[key] = value;

        // Build the update mask.
        FieldMask updateMask = new FieldMask
        {
            Paths = { "labels" }
        };

        // Call the API.
        client.UpdateTemplate(template, updateMask);

        // Get the updated template.
        Template updatedTemplate = client.GetTemplate(templateName);

        // Print the updated labels.
        Console.WriteLine($"Updated regional template {updatedTemplate.Name} with labels {updatedTemplate.Labels}");

        return updatedTemplate;
    }
}
// [END parametermanager_update_regional_param_template_labels]
