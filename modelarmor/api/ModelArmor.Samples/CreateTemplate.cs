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

// [START modelarmor_create_template]
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;
using System.Collections.Generic;

public class CreateTemplateSample
{
    public Template CreateTemplate(
        string projectId = "my-project",
        string locationId = "us-central1",
        string templateId = "my-template"
    )
    {
        ModelArmorClient client = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
        }.Build();

        // Build the Model Armor template with your preferred filters.
        // For more details on filters, please refer to the following doc:
        // https://cloud.google.com/security-command-center/docs/key-concepts-model-armor#ma-filters

        // Configure Responsible AI filter with multiple categories and their confidence
        // levels.
        RaiFilterSettings raiFilterSettings = new RaiFilterSettings();
        List<RaiFilterSettings.Types.RaiFilter> filters =
            new List<RaiFilterSettings.Types.RaiFilter>
            {
                new RaiFilterSettings.Types.RaiFilter
                {
                    FilterType = RaiFilterType.Dangerous,
                    ConfidenceLevel = DetectionConfidenceLevel.High,
                },
                new RaiFilterSettings.Types.RaiFilter
                {
                    FilterType = RaiFilterType.HateSpeech,
                    ConfidenceLevel = DetectionConfidenceLevel.High,
                },
                new RaiFilterSettings.Types.RaiFilter
                {
                    FilterType = RaiFilterType.SexuallyExplicit,
                    ConfidenceLevel = DetectionConfidenceLevel.LowAndAbove,
                },
                new RaiFilterSettings.Types.RaiFilter
                {
                    FilterType = RaiFilterType.Harassment,
                    ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
                },
            };

        raiFilterSettings.RaiFilters.Add(filters);

        Template template = new Template
        {
            FilterConfig = new FilterConfig { RaiSettings = raiFilterSettings },
        };

        CreateTemplateRequest request = new CreateTemplateRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, locationId),
            TemplateId = templateId,
            Template = template,
        };

        Template createdTemplate = client.CreateTemplate(request);
        System.Console.WriteLine($"Created template: {createdTemplate.Name}");

        return createdTemplate;
    }
}
// [END modelarmor_create_template]
