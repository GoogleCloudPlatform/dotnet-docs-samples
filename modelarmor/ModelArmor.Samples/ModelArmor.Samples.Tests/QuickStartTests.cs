/*
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     [https://www.apache.org/licenses/LICENSE-2.0](https://www.apache.org/licenses/LICENSE-2.0)
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class QuickstartTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly ModelArmorClient _client;
        private readonly string _projectId;
        private readonly string _locationId;
        private readonly string _templateIdPrefix;
        private readonly List<string> _templatesToDelete = new List<string>();

        public QuickstartTests(ITestOutputHelper output)
        {
            _output = output;

            // Get location ID from environment variable or use default
            _locationId = Environment.GetEnvironmentVariable("GCLOUD_LOCATION") ?? "us-central1";

            // Create client
            var clientBuilder = new ModelArmorClientBuilder
            {
                Endpoint = $"modelarmor.{_locationId}.rep.googleapis.com"
            };
            _client = clientBuilder.Build();

            // Get project ID (you might need to adjust this based on your authentication setup)
            _projectId = "ma-crest-data-test-2"; // Replace with your project ID or method to get it

            // Create a unique prefix for test templates
            _templateIdPrefix = $"test-template-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        public void Dispose()
        {
            // Clean up all templates after tests
            foreach (var templateName in _templatesToDelete)
            {
                try
                {
                    _client.DeleteTemplate(new DeleteTemplateRequest { Name = templateName });
                    _output.WriteLine($"Cleaned up template: {templateName}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Failed to delete template {templateName}: {ex.Message}");
                }
            }
        }

        [Fact]
        public void ShouldCreateTemplateAndSanitizeContent()
        {
            // Arrange
            var quickstartSample = new QuickstartSample();
            var testQuickstartTemplateId = $"{_templateIdPrefix}-quickstart";

            // Track template for deletion
            _templatesToDelete.Add($"projects/{_projectId}/locations/{_locationId}/templates/{testQuickstartTemplateId}");

            // Act & Assert - if no exception is thrown, the test passes
            quickstartSample.Quickstart(_projectId, _locationId, testQuickstartTemplateId);

            // Additional verification
            var templateName = TemplateName.FromProjectLocationTemplate(_projectId, _locationId, testQuickstartTemplateId);
            var template = _client.GetTemplate(new GetTemplateRequest { Name = templateName.ToString() });

            Assert.NotNull(template);
            Assert.Equal(testQuickstartTemplateId, templateName.TemplateId);
        }
    }
}
