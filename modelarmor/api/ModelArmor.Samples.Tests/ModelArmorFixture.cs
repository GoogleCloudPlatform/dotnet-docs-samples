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
using System.Collections.Generic;
using Google.Cloud.ModelArmor.V1;
using Xunit;

[CollectionDefinition(nameof(ModelArmorFixture))]
public class ModelArmorFixture : IDisposable, ICollectionFixture<ModelArmorFixture>
{
    // Environment variable names.
    private const string EnvProjectId = "GOOGLE_PROJECT_ID";
    private const string EnvLocation = "GOOGLE_CLOUD_LOCATION";

    public ModelArmorClient Client { get; }
    public string ProjectId { get; }
    public string LocationId { get; }
    private readonly List<TemplateName> _resourcesToCleanup = new List<TemplateName>();
    public string InspectTemplateId = "test-dotnet-dlp-inspect-template";
    public string DeidentifyTemplateId = "test-dotnet-dlp-deidentify-template";

    public ModelArmorFixture()
    {
        ProjectId = GetRequiredEnvVar(EnvProjectId);
        LocationId = Environment.GetEnvironmentVariable(EnvLocation) ?? "us-central1";

        // Create the client.
        ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{LocationId}.rep.googleapis.com",
        };
        Client = clientBuilder.Build();
    }

    private string GetRequiredEnvVar(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception($"Missing {name} environment variable");
        }
        return value;
    }

    public string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    public void Dispose()
    {
        // Clean up resources after tests.
        foreach (var resourceName in _resourcesToCleanup)
        {
            try
            {
                Client.DeleteTemplate(new DeleteTemplateRequest { TemplateName = resourceName });
            }
            catch (Exception)
            {
                // Ignore errors during cleanup.
            }
        }
    }

    public void RegisterTemplateForCleanup(TemplateName templateName)
    {
        if (templateName != null && !string.IsNullOrEmpty(templateName.ToString()))
        {
            _resourcesToCleanup.Add(templateName);
        }
    }

    public TemplateName CreateTemplateName(string prefix = "test-dotnet")
    {
        string templateId = $"{prefix}-{GenerateUniqueId()}";
        return new TemplateName(ProjectId, LocationId, templateId);
    }
}
