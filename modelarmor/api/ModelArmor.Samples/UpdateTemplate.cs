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

// [START modelarmor_update_template]
using System;
using System.Collections.Generic;
using Google.Cloud.ModelArmor.V1;
using Google.Protobuf.WellKnownTypes;

namespace ModelArmor.Samples
{
    public class UpdateTemplateSample
    {
        public Template UpdateTemplate(
            string projectId = "my-project",
            string locationId = "us-central1",
            string templateId = "my-template"
        )
        {
            // Construct the API endpoint URL.
            ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
            {
                Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
            };

            // Create the client.
            ModelArmorClient client = clientBuilder.Build();

            // Get the template name.
            TemplateName name = TemplateName.FromProjectLocationTemplate(
                projectId,
                locationId,
                templateId
            );

            // Build the updated Model Armor template with modified filters.
            // For more details on filters, please refer to the following doc:
            // https://cloud.google.com/security-command-center/docs/key-concepts-model-armor#ma-filters
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
                        ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
                    },
                    new RaiFilterSettings.Types.RaiFilter
                    {
                        FilterType = RaiFilterType.SexuallyExplicit,
                        ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
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
                TemplateName = name,
                FilterConfig = new FilterConfig { RaiSettings = raiFilterSettings },
            };

            // Create a field mask to specify which fields to update.
            // Ref: https://protobuf.dev/reference/protobuf/google.protobuf/#field-mask
            FieldMask updateMask = new FieldMask { Paths = { "filter_config.rai_settings" } };

            // Prepare the request.
            UpdateTemplateRequest request = new UpdateTemplateRequest
            {
                Template = template,
                UpdateMask = updateMask,
            };

            // Send the request.
            Template updatedTemplate = client.UpdateTemplate(request);
            Console.WriteLine($"Updated template: {updatedTemplate.Name}");

            return updatedTemplate;
        }
    }
}
// [END modelarmor_update_template]
