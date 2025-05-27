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
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class UpdateTemplateTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateSample _create_template_sample;
        private readonly UpdateTemplateSample _update_template_sample;

        public UpdateTemplateTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _create_template_sample = new CreateTemplateSample();
            _update_template_sample = new UpdateTemplateSample();
        }

        [Fact]
        public void UpdateTemplateTest()
        {
            // Create a template.
            TemplateName templateName = _fixture.CreateTemplateName();

            Template originalTemplate = _create_template_sample.CreateTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateName.TemplateId
            );

            string templateId = TemplateName.Parse(originalTemplate.Name).TemplateId;

            // Call the sample to modify the RAI filter of above created template.
            Template updatedTemplate = _update_template_sample.UpdateTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateId
            );

            // Assertions.
            Assert.NotNull(updatedTemplate);
            Assert.Equal(originalTemplate.Name, updatedTemplate.Name);

            // Verify specific RAI filters were updated.
            Assert.NotNull(updatedTemplate.FilterConfig);
            Assert.NotNull(updatedTemplate.FilterConfig.RaiSettings);

            var raiFilters = updatedTemplate.FilterConfig.RaiSettings.RaiFilters;
            Assert.Contains(raiFilters, f => f.FilterType == RaiFilterType.Dangerous);
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.Dangerous
                    && f.ConfidenceLevel == DetectionConfidenceLevel.High
            );

            Assert.Contains(raiFilters, f => f.FilterType == RaiFilterType.HateSpeech);
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.HateSpeech
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );

            Assert.Contains(raiFilters, f => f.FilterType == RaiFilterType.Harassment);
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.Harassment
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );

            Assert.Contains(raiFilters, f => f.FilterType == RaiFilterType.SexuallyExplicit);
            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.SexuallyExplicit
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );

            _fixture.RegisterTemplateForCleanup(templateName);
        }

        [Fact]
        public void UpdateTemplateTest_NonExistentTemplate_ThrowsException()
        {
            // Generate a random template ID that shouldn't exist.
            string nonExistentTemplateId = $"non-existent-{Guid.NewGuid():N}";

            // Attempt to update a non-existent template should throw an exception.
            var exception = Assert.Throws<Grpc.Core.RpcException>(() =>
                _update_template_sample.UpdateTemplate(
                    projectId: _fixture.ProjectId,
                    locationId: _fixture.LocationId,
                    templateId: nonExistentTemplateId
                )
            );

            // Verify the exception contains the expected error message.
            Assert.Contains(
                nonExistentTemplateId,
                exception.Message,
                StringComparison.OrdinalIgnoreCase
            );
            Assert.Contains("NOT FOUND", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
