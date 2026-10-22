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

// [START parametermanager_create_param_template_version]

using Google.Cloud.ParameterManager.V1;
using Google.Protobuf;
using System.Text;

public class CreateParameterTemplateVersionSample
{
    /// <summary>
    /// Creates a template version with placeholders in its payload.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="templateId">The ID of the template.</param>
    /// <param name="versionId">The ID of the template version.</param>
    public TemplateVersion CreateParameterTemplateVersion(
        string projectId,
        string templateId,
        string versionId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the parent resource name.
        TemplateName parent = new TemplateName(projectId, "global", templateId);

        // Build the template version with placeholders in its payload.
        string payload = "{\"username\": \"{{.username}}\", \"host\": \"{{.host}}\"}";
        TemplateVersion templateVersion = new TemplateVersion
        {
            Payload = new TemplateVersionPayload
            {
                Data = ByteString.CopyFrom(payload, Encoding.UTF8)
            }
        };

        // Call the API.
        TemplateVersion createdTemplateVersion = client.CreateTemplateVersion(parent, templateVersion, versionId);

        // Print the created template version name.
        Console.WriteLine($"Created template version: {createdTemplateVersion.Name}");

        return createdTemplateVersion;
    }
}
// [END parametermanager_create_param_template_version]
