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

// [START parametermanager_render_param_template_version]

using Google.Cloud.ParameterManager.V1;
using System.Text;

public class RenderParameterTemplateVersionSample
{
    /// <summary>
    /// Renders a template version with the values of a parameter version.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="templateId">The ID of the template.</param>
    /// <param name="versionId">The ID of the template version.</param>
    /// <param name="parameterId">The ID of the parameter.</param>
    /// <param name="parameterVersionId">The ID of the parameter version.</param>
    public RenderTemplateVersionResponse RenderParameterTemplateVersion(
        string projectId,
        string templateId,
        string versionId,
        string parameterId,
        string parameterVersionId)
    {
        // Create the client.
        ParameterManagerClient client = ParameterManagerClient.Create();

        // Build the request.
        RenderTemplateVersionRequest request = new RenderTemplateVersionRequest
        {
            TemplateVersionName = new TemplateVersionName(projectId, "global", templateId, versionId),
            ParameterVersionAsParameterVersionName = new ParameterVersionName(projectId, "global", parameterId, parameterVersionId)
        };

        // Call the API.
        RenderTemplateVersionResponse response = client.RenderTemplateVersion(request);

        // Print the template payload and the rendered payload.
        Console.WriteLine($"Template payload: {Encoding.UTF8.GetString(response.Payload.Data.ToByteArray())}");
        Console.WriteLine($"Rendered payload: {Encoding.UTF8.GetString(response.RenderedPayload.ToByteArray())}");

        return response;
    }
}
// [END parametermanager_render_param_template_version]
