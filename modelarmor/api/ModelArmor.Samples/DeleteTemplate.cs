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

// [START modelarmor_delete_template]
using System;
using Google.Cloud.ModelArmor.V1;

public class DeleteTemplateSample
{
    public void DeleteTemplate(
        string projectId = "my-project",
        string locationId = "us-central1",
        string templateId = "my-template"
    )
    {
        ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
        };

        ModelArmorClient client = clientBuilder.Build();

        TemplateName name = TemplateName.FromProjectLocationTemplate(
            projectId,
            locationId,
            templateId
        );

        DeleteTemplateRequest request = new DeleteTemplateRequest { TemplateName = name };

        client.DeleteTemplate(request);
        Console.WriteLine($"Deleted template: {name}");
    }
}
// [END modelarmor_delete_template]
