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
    public class UpdateTemplateWithLabelsTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateSample _create_template_sample;
        private readonly GetTemplateSample _get_template_sample;
        private readonly UpdateTemplateWithLabelsSample _update_template_sample;

        public UpdateTemplateWithLabelsTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _create_template_sample = new CreateTemplateSample();
            _get_template_sample = new GetTemplateSample();
            _update_template_sample = new UpdateTemplateWithLabelsSample();
        }

        [Fact]
        public void UpdateTemplateWithLabelsTest()
        {
            // Create a template.
            TemplateName templateName = _fixture.CreateTemplateName();
            _fixture.RegisterTemplateForCleanup(templateName);

            Template originalTemplate = _create_template_sample.CreateTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateName.TemplateId
            );

            string templateId = TemplateName.Parse(originalTemplate.Name).TemplateId;

            Template updatedTemplate = _update_template_sample.UpdateTemplateWithLabels(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateId
            );

            Assert.NotNull(updatedTemplate);
            Assert.Equal(originalTemplate.Name, updatedTemplate.Name);

            // Get template details for assertion of labels.
            Template getUpdatedTemplate = _get_template_sample.GetTemplate(
                projectId: _fixture.ProjectId,
                locationId: _fixture.LocationId,
                templateId: templateName.TemplateId
            );

            Assert.NotNull(getUpdatedTemplate.Labels);
            Assert.Contains(
                getUpdatedTemplate.Labels,
                l => l.Key == "updated_key1" && l.Value == "updated_value1"
            );
            Assert.Contains(
                getUpdatedTemplate.Labels,
                l => l.Key == "updated_key2" && l.Value == "updated_value2"
            );
        }
    }
}
