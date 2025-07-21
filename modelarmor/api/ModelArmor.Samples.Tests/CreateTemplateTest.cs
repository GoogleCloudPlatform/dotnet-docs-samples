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
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class CreateTemplateTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateSample _sample;

        public CreateTemplateTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _sample = new CreateTemplateSample();
        }

        [Fact]
        public void Runs()
        {
            TemplateName templateName = _fixture.CreateTemplateName();
            _fixture.RegisterTemplateForCleanup(templateName);
            string templateId = templateName.TemplateId;

            // Run the sample.
            Template template = _sample.CreateTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateId
            );

            // Verify the template was created successfully.
            Assert.NotNull(template);
            Assert.Contains(templateId, template.Name);

            // Verify the template has the expected filter configuration.
            Assert.NotNull(template.FilterConfig);
            Assert.NotNull(template.FilterConfig.RaiSettings);
            var raiFilters = template.FilterConfig.RaiSettings.RaiFilters;
            Assert.Equal(4, raiFilters.Count);

            // Assert each filter exists with the expected configuration.
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
        }
    }
}
