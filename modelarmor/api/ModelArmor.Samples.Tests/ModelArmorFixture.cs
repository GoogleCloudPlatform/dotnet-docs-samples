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

    /// <summary>
    /// Gets an environment variable or throws an exception if it is not defined.
    /// </summary>
    /// <param name="name">The name of the environment variable.</param>
    /// <returns>The value of the environment variable.</returns>
    /// <exception cref="Exception">Thrown if the environment variable is not defined.</exception>
    private string GetRequiredEnvVar(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception($"Missing {name} environment variable");
        }
        return value;
    }

    /// <summary>
    /// Generates a unique identifier. This identifier is used to ensure
    /// that resources created by tests are unique and can be cleaned up
    /// successfully.
    /// </summary>
    /// <returns>A unique identifier.</returns>
    public string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    /// <summary>
    /// Clean up resources after tests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is called automatically by the test framework after all tests
    /// in the test class have been completed. It is not intended to be called
    /// directly.
    /// </para>
    /// <para>
    /// This method is implemented according to the <see cref="IDisposable"/> interface.
    /// </para>
    /// </remarks>
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

    /// <summary>
    /// Registers a template for cleanup. The template will be deleted after the test fixture is disposed.
    /// </summary>
    /// <param name="templateName">The name of the template to register.</param>
    public void RegisterTemplateForCleanup(TemplateName templateName)
    {
        if (templateName != null && !string.IsNullOrEmpty(templateName.ToString()))
        {
            _resourcesToCleanup.Add(templateName);
        }
    }

    /// <summary>
    /// Creates a template name from the provided prefix and a random, unique suffix.
    /// </summary>
    /// <param name="prefix">The prefix to use for the template name.</param>
    /// <returns>The created template name.</returns>
    public TemplateName CreateTemplateName(string prefix = "test-dotnet")
    {
        string templateId = $"{prefix}-{GenerateUniqueId()}";
        return new TemplateName(ProjectId, LocationId, templateId);
    }


    /// <summary>
    /// Gets a template by name.
    /// </summary>
    /// <param name="templateName">The name of the template to get.</param>
    /// <returns>The template if found, otherwise null.</returns>
    public Template GetTemplate(TemplateName templateName)
    {
        GetTemplateRequest request = new GetTemplateRequest { TemplateName = templateName };

        return Client.GetTemplate(request);
    }
}
