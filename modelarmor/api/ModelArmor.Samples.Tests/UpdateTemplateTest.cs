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

using Google.Cloud.ModelArmor.V1;
using System;
using System.Collections.Generic;
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
            _fixture.RegisterTemplateForCleanup(templateName);

            Template originalTemplate = _create_template_sample.CreateTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateName.TemplateId);

            string templateId = templateName.TemplateId;

            // Call the sample to modify the RAI filter of above created template.
            Template updatedTemplate = _update_template_sample.UpdateTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateId);

            // Assertions.
            var raiFilters = updatedTemplate.FilterConfig.RaiSettings.RaiFilters;
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
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );

            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.Harassment
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );

            Assert.Contains(
                raiFilters,
                f =>
                    f.FilterType == RaiFilterType.SexuallyExplicit
                    && f.ConfidenceLevel == DetectionConfidenceLevel.MediumAndAbove
            );
        }
    }
}
