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
using System.Linq;
using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class CreateTemplateWithMetadataTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateWithMetadataSample _sample;
        private readonly ITestOutputHelper _output;

        public CreateTemplateWithMetadataTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _sample = new CreateTemplateWithMetadataSample();
            _output = output;
        }

        [Fact]
        public void CreateTemplateWithMetadataTest()
        {
            string projectId = _fixture.ProjectId;
            string locationId = _fixture.LocationId;

            // Generate a unique template ID for testing
            string templateId = $"test-metadata-{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            _output.WriteLine($"Creating template with metadata: {templateId}");

            // Run the sample
            Template createdTemplate = _sample.CreateTemplateWithMetadata(
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

            // Verify RAI filter settings - directly assert the presence of each expected filter
            var raiFilters = createdTemplate.FilterConfig.RaiSettings.RaiFilters;

            // Assert each filter exists with the expected configuration
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.Dangerous
                    && f.ConfidenceLevel == DetectionConfidenceLevel.High
            );

            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.HateSpeech
                    && f.ConfidenceLevel == DetectionConfidenceLevel.High
            );

            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.SexuallyExplicit
                    && f.ConfidenceLevel == DetectionConfidenceLevel.LowAndAbove
            );

            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.Harassment
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );

            // Verify the template metadata
            Assert.NotNull(createdTemplate.TemplateMetadata);

            // Verify the metadata
            Assert.NotNull(createdTemplate.TemplateMetadata);
            Assert.True(createdTemplate.TemplateMetadata.IgnorePartialInvocationFailures);
            Assert.True(createdTemplate.TemplateMetadata.LogSanitizeOperations);
            Assert.Equal(500, createdTemplate.TemplateMetadata.CustomPromptSafetyErrorCode);

            // Clean up - delete the template
            try
            {
                _fixture.Client.DeleteTemplate(createdTemplate.Name);
                _output.WriteLine($"Deleted template: {createdTemplate.Name}");
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error during cleanup: {ex.Message}");
                // Don't fail the test if cleanup fails
            }
        }
    }
}
