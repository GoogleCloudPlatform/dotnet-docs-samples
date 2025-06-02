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

// [START modelarmor_sanitize_model_response]
using System;
using Google.Cloud.ModelArmor.V1;

namespace ModelArmor.Samples
{
    public class SanitizeModelResponseSample
    {
        public SanitizeModelResponseResponse SanitizeModelResponse(
            string projectId = "my-project",
            string locationId = "us-central1",
            string templateId = "my-template",
            string modelResponse = "Unsanitized model output"
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

            // Prepare the request.
            SanitizeModelResponseRequest request = new SanitizeModelResponseRequest
            {
                TemplateName = templateName,
                ModelResponseData = new DataItem { Text = modelResponse },
            };

            // Send the request and get the response.
            SanitizeModelResponseResponse response = client.SanitizeModelResponse(request);

            // Print the sanitization result
            Console.WriteLine($"Result for the provided model response: {response}");

            return response;
        }
    }
}
// [END modelarmor_sanitize_model_response]
