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
using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;

public class QuickstartSample
{
    public void Quickstart(
        string projectId = "my-project",
        string locationId = "us-central1",
        string templateId = "my-template"
    )
    {
        ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
        };

        // Create the client.
        Google.Cloud.ModelArmor.V1.ModelArmorClient client = clientBuilder.Build();

        LocationName parent = LocationName.FromProjectLocation(projectId, locationId);

        // Build the Model Armor template with preferred filters
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
                ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
            }
        );
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.SexuallyExplicit,
                ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
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

        // Create the template
        Template template = new Template { FilterConfig = modelArmorFilter };

        // Create the request
        CreateTemplateRequest request = new CreateTemplateRequest
        {
            ParentAsLocationName = parent,
            TemplateId = templateId,
            Template = template,
        };

        // Send the request
        Template createdTemplate = client.CreateTemplate(request);

        Console.WriteLine($"Created template: {createdTemplate.Name}");

        // Sanitize a user prompt using the created template
        string userPrompt = "Unsafe user prompt";

        TemplateName templateName = TemplateName.FromProjectLocationTemplate(
            projectId,
            locationId,
            templateId
        );

        SanitizeUserPromptRequest userPromptSanitizeRequest = new SanitizeUserPromptRequest
        {
            TemplateName = templateName,
            UserPromptData = new DataItem { Text = userPrompt },
        };

        SanitizeUserPromptResponse userPromptSanitizeResponse = client.SanitizeUserPrompt(
            userPromptSanitizeRequest
        );

        Console.WriteLine($"Result for User Prompt Sanitization: {userPromptSanitizeResponse}");

        // Sanitize a model response using the created template
        string modelResponse = "Unsanitized model output";

        SanitizeModelResponseRequest modelSanitizeRequest = new SanitizeModelResponseRequest
        {
            TemplateName = templateName,
            ModelResponseData = new DataItem { Text = modelResponse },
        };

        SanitizeModelResponseResponse modelSanitizeResponse = client.SanitizeModelResponse(
            modelSanitizeRequest
        );

        Console.WriteLine($"Result for Model Response Sanitization: {modelSanitizeResponse}");
    }
}
// [END modelarmor_quickstart]
