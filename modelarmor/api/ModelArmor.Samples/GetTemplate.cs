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

// [START modelarmor_get_template]
using System;
using Google.Cloud.ModelArmor.V1;

namespace ModelArmor.Samples
{
    public class GetTemplateSample
    {
        public Template GetTemplate(
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

            TemplateName name = TemplateName.FromProjectLocationTemplate(
                projectId,
                locationId,
                templateId
            );

            // Prepare the request.
            GetTemplateRequest request = new GetTemplateRequest { TemplateName = name };

            // Call the API to get the template.
            Template template = client.GetTemplate(request);

            // Print template details.
            Console.WriteLine($"Retrieved template: {template.Name}");

            return template;
        }
    }
}
// [END modelarmor_get_template]
