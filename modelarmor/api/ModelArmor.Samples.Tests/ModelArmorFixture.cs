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

using System;
using Google.Cloud.ModelArmor.V1;
using Xunit;

[CollectionDefinition(nameof(ModelArmorFixture))]
public class ModelArmorFixture : IDisposable, ICollectionFixture<ModelArmorFixture>
{
    public ModelArmorClient Client { get; }
    public string ProjectId { get; }
    public string LocationId { get; }
    public TemplateName TemplateForQuickstartName { get; }

    public ModelArmorFixture()
    {
        // Get the Google Cloud Project ID.
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("Missing GOOGLE_PROJECT_ID environment variable");
        }

        // Get location ID from environment variable or use default.
        LocationId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_LOCATION") ?? "us-central1";

        // Create client.
        Client = ModelArmorClient.Create();

        // Create a template name for quickstart test.
        string templateId = $"test-csharp-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        TemplateForQuickstartName = TemplateName.FromProjectLocationTemplate(
            ProjectId,
            LocationId,
            templateId
        );
    }

    public void Dispose()
    {
        // Clean up resources after tests.
        try
        {
            Client.DeleteTemplate(
                new DeleteTemplateRequest { Name = TemplateForQuickstartName.ToString() }
            );
        }
        catch (Exception)
        {
            // Ignore errors during cleanup.
        }
    }
}
