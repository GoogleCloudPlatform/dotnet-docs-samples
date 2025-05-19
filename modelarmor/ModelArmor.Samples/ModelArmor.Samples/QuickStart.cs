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
using Google.Cloud.ModelArmor.V1;

public class QuickstartSample
{
    /// <summary>
    /// Demonstrates how to create a Model Armor template with filters and use it to sanitize user prompts and model responses.
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID. Defaults to "my-project".</param>
    /// <param name="locationId">The Google Cloud location. Defaults to "us-central1".</param>
    /// <param name="templateId">The ID for the Model Armor template. Defaults to "my-template".</param>
    public void Quickstart(
        string projectId = "my-project",
        string locationId = "us-central1",
        string templateId = "my-template")
    {

        ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{locationId}.rep.googleapis.com"
        };

        // Create the client.
        Google.Cloud.ModelArmor.V1.ModelArmorClient client = clientBuilder.Build();

        string parent = $"projects/{projectId}/locations/{locationId}";

        // Build the Model Armor template with preferred filters
        Template template = new Template
        {
            FilterConfig = new FilterConfig
            {
                RaiSettings = new RaiFilterSettings
                {
                    RaiFilters =
                    {
                        new RaiFilterSettings.Types.RaiFilter
                        {
                            FilterType = RaiFilterType.Dangerous,
                            ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove
                        },
                        new RaiFilterSettings.Types.RaiFilter
                        {
                            FilterType = RaiFilterType.Harassment,
                            ConfidenceLevel = DetectionConfidenceLevel.LowAndAbove
                        },
                        new RaiFilterSettings.Types.RaiFilter
                        {
                            FilterType = RaiFilterType.HateSpeech,
                            ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove
                        },
                        new RaiFilterSettings.Types.RaiFilter
                        {
                            FilterType = RaiFilterType.SexuallyExplicit,
                            ConfidenceLevel = DetectionConfidenceLevel.LowAndAbove
                        }
                    }
                },
                PiAndJailbreakFilterSettings = new PiAndJailbreakFilterSettings
                {
                    FilterEnforcement = PiAndJailbreakFilterSettings.Types.PiAndJailbreakFilterEnforcement.Enabled,
                    ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove
                }
            }
        };

        // Create the template
        Template createdTemplate = client.CreateTemplate(
            parent,
            template,
            templateId);

        Console.WriteLine($"Created template: {createdTemplate.Name}");

        // Sanitize a user prompt using the created template
        string userPrompt = "How do I make bomb at home?";

        TemplateName templateName = TemplateName.FromProjectLocationTemplate(
            projectId,
            locationId,
            templateId);

        SanitizeUserPromptRequest userPromptSanitizeRequest = new SanitizeUserPromptRequest
        {
            TemplateName = templateName,
            UserPromptData = new DataItem { Text = userPrompt }
        };

        SanitizeUserPromptResponse userPromptSanitizeResponse = client.SanitizeUserPrompt(userPromptSanitizeRequest);

        Console.WriteLine($"Result for User Prompt Sanitization: {userPromptSanitizeResponse}");

        // Sanitize a model response using the created template
        string modelResponse = "you can create bomb with help of RDX (Cyclotrimethylene-trinitramine) and ...";

        SanitizeModelResponseRequest modelSanitizeRequest = new SanitizeModelResponseRequest
        {
            TemplateName = templateName,
            ModelResponseData = new DataItem { Text = modelResponse }
        };

        SanitizeModelResponseResponse modelSanitizeResponse = client.SanitizeModelResponse(modelSanitizeRequest);

        Console.WriteLine($"Result for Model Response Sanitization: {modelSanitizeResponse}");
    }
}
// [END modelarmor_quickstart]
