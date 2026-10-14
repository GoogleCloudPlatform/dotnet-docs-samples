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

// [START parametermanager_regional_template_quickstart]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ParameterManager.V1;
using Google.Protobuf;
using System.Text;

public class RegionalTemplateQuickstartSample
{
    /// <summary>
    /// Creates a regional template, a parameter version and renders the template version.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="locationId">The ID of the region.</param>
    /// <param name="templateId">The ID of the template.</param>
    /// <param name="parameterId">The ID of the parameter.</param>
    /// <param name="versionId">The ID used for both the template version and the parameter version.</param>
    public void RegionalTemplateQuickstart(
        string projectId,
        string locationId,
        string templateId,
        string parameterId,
        string versionId)
    {
        // Define the regional endpoint.
        string regionalEndpoint = $"parametermanager.{locationId}.rep.googleapis.com";

        // Create the client with the regional endpoint.
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();

        // Build the parent resource name.
        LocationName parent = new LocationName(projectId, locationId);

        // Create a template in JSON format.
        Template template = client.CreateTemplate(parent, new Template { Format = TemplateFormat.Json }, templateId);
        Console.WriteLine($"Created regional template: {template.Name}");

        // Create a template version with placeholders.
        string templatePayload = "{\"username\": \"{{.username}}\", \"host\": \"{{.host}}\"}";
        TemplateVersion templateVersion = client.CreateTemplateVersion(
            template.TemplateName,
            new TemplateVersion
            {
                Payload = new TemplateVersionPayload
                {
                    Data = ByteString.CopyFrom(templatePayload, Encoding.UTF8)
                }
            },
            versionId);
        Console.WriteLine($"Created regional template version: {templateVersion.Name}");

        // Create a JSON parameter.
        Parameter parameter = client.CreateParameter(parent, new Parameter { Format = ParameterFormat.Json }, parameterId);
        Console.WriteLine($"Created regional parameter: {parameter.Name}");

        // Create a parameter version with the values for the placeholders.
        string values = "{\"username\": \"test-user\", \"host\": \"localhost\"}";
        ParameterVersion parameterVersion = client.CreateParameterVersion(
            parameter.ParameterName,
            new ParameterVersion
            {
                Payload = new ParameterVersionPayload
                {
                    Data = ByteString.CopyFrom(values, Encoding.UTF8)
                }
            },
            versionId);
        Console.WriteLine($"Created regional parameter version: {parameterVersion.Name}");

        // Render the template version with the parameter version values.
        RenderTemplateVersionResponse response = client.RenderTemplateVersion(
            new RenderTemplateVersionRequest
            {
                TemplateVersionName = templateVersion.TemplateVersionName,
                ParameterVersionAsParameterVersionName = parameterVersion.ParameterVersionName
            });
        Console.WriteLine($"Rendered payload: {Encoding.UTF8.GetString(response.RenderedPayload.ToByteArray())}");
    }
}
// [END parametermanager_regional_template_quickstart]
