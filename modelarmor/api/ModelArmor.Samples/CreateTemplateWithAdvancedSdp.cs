/*
 * Copyright 2025 Google LLC
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

// [START modelarmor_create_template_with_advanced_sdp]
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;
using System;

public class CreateTemplateWithAdvancedSdpSample
{
    public Template CreateTemplateWithAdvancedSdp(
        string projectId = "my-project",
        string locationId = "us-central1",
        string templateId = "my-template",
        string inspectTemplateName =
            "projects/my_project/locations/us-central1/inspectTemplates/inspect_template_id",
        string deidentifyTemplateName =
            "projects/my_project/locations/us-central1/deidentifyTemplates/de-identify_template_id"
    )
    {
        ModelArmorClient client = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
        }.Build();

        LocationName parent = LocationName.FromProjectLocation(projectId, locationId);

        // Build the Model Armor template with Advanced SDP Filter.

        // Note: If you specify only Inspect template, Model Armor reports the filter matches if
        // sensitive data is detected. If you specify Inspect template and De-identify template, Model
        // Armor returns the de-identified sensitive data and sanitized version of prompts or
        // responses in the deidentifyResult.data.text field of the finding.
        SdpAdvancedConfig advancedSdpConfig = new SdpAdvancedConfig
        {
            InspectTemplate = inspectTemplateName,
            DeidentifyTemplate = deidentifyTemplateName,
        };

        SdpFilterSettings sdpSettings = new SdpFilterSettings
        {
            AdvancedConfig = advancedSdpConfig,
        };

        FilterConfig filterConfig = new FilterConfig { SdpSettings = sdpSettings };
        Template template = new Template { FilterConfig = filterConfig };

        CreateTemplateRequest request = new CreateTemplateRequest
        {
            ParentAsLocationName = parent,
            TemplateId = templateId,
            Template = template,
        };

        Template createdTemplate = client.CreateTemplate(request);

        Console.WriteLine($"Created template with Advanced SDP filter: {createdTemplate.Name}");

        return createdTemplate;
    }
}
// [END modelarmor_create_template_with_advanced_sdp]
