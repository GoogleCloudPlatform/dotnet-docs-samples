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

// [START modelarmor_list_templates]
using System;
using System.Collections.Generic;
using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;

namespace ModelArmor.Samples
{
    public class ListTemplatesSample
    {
        public IEnumerable<Template> ListTemplates(
            string projectId = "my-project",
            string locationId = "us-central1"
        )
        {
            // Construct the API endpoint URL.
            ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
            {
                Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
            };

            // Create the client.
            ModelArmorClient client = clientBuilder.Build();

            // Build the parent resource name.
            LocationName parent = LocationName.FromProjectLocation(projectId, locationId);

            // Create the list request.
            ListTemplatesRequest request = new ListTemplatesRequest
            {
                ParentAsLocationName = parent,
            };

            // List all templates.
            PagedEnumerable<ListTemplatesResponse, Template> response = client.ListTemplates(
                request
            );

            // Display the templates.
            foreach (Template template in response)
            {
                Console.WriteLine($"Template: {template.Name}");
            }

            return response;
        }
    }
}
// [END modelarmor_list_templates]
