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

// [START modelarmor_sanitize_prompt_and_response]
using Google.Cloud.ModelArmor.V1;
using ModelArmor.Samples;

public class SanitizeModelResponseWithUserPromptSample
{
    public SanitizeModelResponseResponse SanitizeModelResponseWithUserPrompt(
        string projectId = "my-project",
        string locationId = "us-central1",
        string templateId = "my-template",
        string userPrompt = "My email is user@example.com and my phone is 555-123-4567",
        string modelResponse = "I found your ITIN: 988-86-1234 in our records"
    )
    {
        // Endpoint to call the Model Armor server.
        ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
        };

        // Create the client.
        ModelArmorClient client = clientBuilder.Build();

        // Build the resource name of the template.
        TemplateName templateName = TemplateName.FromProjectLocationTemplate(
            projectId,
            locationId,
            templateId
        );

        // Prepare and send the user prompt sanitization request
        SanitizeModelResponseRequest request = new SanitizeModelResponseRequest
        {
            TemplateName = TemplateName.FromProjectLocationTemplate(
                projectId,
                locationId,
                templateId
            ),
            ModelResponseData = new DataItem { Text = modelResponse },
            UserPrompt = userPrompt,
        };

        SanitizeModelResponseResponse modelResponseResult = client.SanitizeModelResponse(request);

        return modelResponseResult;
    }
}
// [END modelarmor_sanitize_prompt_and_response]
