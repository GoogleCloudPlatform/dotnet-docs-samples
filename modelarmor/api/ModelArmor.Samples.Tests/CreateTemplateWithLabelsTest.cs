// Copyright(c) 2024 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Collections.Generic;
using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class CreateTemplateWithLabelsTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateWithLabelsSample _sample;
        private readonly ITestOutputHelper _output;

        public CreateTemplateWithLabelsTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _sample = new CreateTemplateWithLabelsSample();
            _output = output;
        }

        [Fact]
        public void CreateTemplateWithLabelsTest()
        {
            string projectId = _fixture.ProjectId;
            string locationId = _fixture.LocationId;

            // Generate a unique template ID for testing
            string templateId = $"test-labels-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            _output.WriteLine($"Creating template with labels: {templateId}");

            // Run the sample
            Template createdTemplate = _sample.CreateTemplateWithLabels(
                projectId: projectId,
                locationId: locationId,
                templateId: templateId
            );

            // Output template details
            _output.WriteLine($"Created template: {createdTemplate.Name}");

            // Verify the template was created successfully
            Assert.NotNull(createdTemplate);
            Assert.Contains(templateId, createdTemplate.Name);

            // Verify the template has the expected filter configuration
            Assert.NotNull(createdTemplate.FilterConfig);
            Assert.NotNull(createdTemplate.FilterConfig.RaiSettings);
            Assert.Equal(4, createdTemplate.FilterConfig.RaiSettings.RaiFilters.Count);

            // Since labels might not be returned in the creation response,
            // we need to retrieve the template to verify labels
            ModelArmorClientBuilder clientBuilder = new ModelArmorClientBuilder
            {
                Endpoint = $"modelarmor.{locationId}.rep.googleapis.com",
            };
            ModelArmorClient client = clientBuilder.Build();

            // Get the template to verify labels
            Template retrievedTemplate = client.GetTemplate(createdTemplate.Name);
            _output.WriteLine($"Retrieved template: {retrievedTemplate.Name}");

            // Verify the labels
            Assert.NotNull(retrievedTemplate.Labels);
            Assert.Equal(2, retrievedTemplate.Labels.Count);
            Assert.Equal("value1", retrievedTemplate.Labels["key1"]);
            Assert.Equal("value2", retrievedTemplate.Labels["key2"]);

            // Clean up - delete the template
            try
            {
                client.DeleteTemplate(retrievedTemplate.Name);
                _output.WriteLine($"Deleted template: {retrievedTemplate.Name}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error during cleanup: {ex.Message}");
                // Don't fail the test if cleanup fails
            }
        }
    }
}
