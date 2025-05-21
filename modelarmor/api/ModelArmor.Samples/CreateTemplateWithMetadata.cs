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

//[START modelarmor_quickstart]
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;

public class CreateTemplateWithMetadataSample
{
    public Template CreateTemplateWithMetadata(
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
        Google.Cloud.ModelArmor.V1.ModelArmorClient client = clientBuilder.Build();

        // Build the parent resource name.
        LocationName parent = LocationName.FromProjectLocation(projectId, locationId);

        // Build the Model Armor template with RAI filters
        RaiFilterSettings raiFilterSettings = new RaiFilterSettings();
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.Dangerous,
                ConfidenceLevel = DetectionConfidenceLevel.High,
            }
        );
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.HateSpeech,
                ConfidenceLevel = DetectionConfidenceLevel.High,
            }
        );
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.SexuallyExplicit,
                ConfidenceLevel = DetectionConfidenceLevel.LowAndAbove,
            }
        );
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.Harassment,
                ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
            }
        );

        // Create the filter config with RAI settings
        FilterConfig modelArmorFilter = new FilterConfig { RaiSettings = raiFilterSettings };

        // For more details about metadata, refer to the following documentation:
        // https://cloud.google.com/security-command-center/docs/reference/model-armor/rest/v1/projects.locations.templates#templatemetadata
        Template.Types.TemplateMetadata templateMetadata = new Template.Types.TemplateMetadata
        {
            IgnorePartialInvocationFailures = true,
            LogSanitizeOperations = true,
            CustomPromptSafetyErrorCode = 500,
        };

        // Create the template with filter config and metadata
        Template template = new Template
        {
            FilterConfig = modelArmorFilter,
            TemplateMetadata = templateMetadata,
        };

        // Create the request
        CreateTemplateRequest request = new CreateTemplateRequest
        {
            ParentAsLocationName = parent,
            TemplateId = templateId,
            Template = template,
        };

        // Send the request
        Template createdTemplate = client.CreateTemplate(request);
        System.Console.WriteLine(
            $"Successfully created template with name: {createdTemplate.Name}"
        );
        return createdTemplate;
    }
}
// [END modelarmor_create_template]
