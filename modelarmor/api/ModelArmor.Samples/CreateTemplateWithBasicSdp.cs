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

// [START modelarmor_create_template_with_basic_sdp]
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;
using System;

public class CreateTemplateWithBasicSdpSample
{
    public Template CreateTemplateWithBasicSdp(
        string projectId = "my-project",
        string locationId = "us-central1",
        string templateId = "my-template")
    {
        ModelArmorClient client = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
        }.Build();

        LocationName parent = LocationName.FromProjectLocation(projectId, locationId);

        // Build the Model Armor template with Basic SDP Filter.
        // For more details on filters, please refer to:
        // https://cloud.google.com/security-command-center/docs/key-concepts-model-armor#ma-filters
        SdpBasicConfig basicSdpConfig = new SdpBasicConfig
        {
            FilterEnforcement = SdpBasicConfig.Types.SdpBasicConfigEnforcement.Enabled,
        };

        SdpFilterSettings sdpSettings = new SdpFilterSettings { BasicConfig = basicSdpConfig };

        FilterConfig filterConfig = new FilterConfig { SdpSettings = sdpSettings };

        CreateTemplateRequest request = new CreateTemplateRequest
        {
            ParentAsLocationName = parent,
            TemplateId = templateId,
            Template = new Template { FilterConfig = filterConfig },
        };

        Template createdTemplate = client.CreateTemplate(request);

        Console.WriteLine($"Created template with Basic SDP filter: {createdTemplate.Name}");

        return createdTemplate;
    }
}
// [END modelarmor_create_template_with_basic_sdp]
