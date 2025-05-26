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

using Google.Cloud.Dlp.V2;
using Google.Cloud.ModelArmor.V1;
using Xunit;

namespace ModelArmor.Samples.Tests
{
    public class CreateTemplateWithAdvancedSdpTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly CreateTemplateWithAdvancedSdpSample _sample;

        public CreateTemplateWithAdvancedSdpTests(ModelArmorFixture fixture)
        {
            _fixture = fixture;
            _sample = new CreateTemplateWithAdvancedSdpSample();
        }

        [Fact]
        public void CreateTemplateWithAdvancedSdpTest()
        {
            string inspectTemplateId = _fixture.InspectTemplateId;
            string deidentifyTemplateId = _fixture.DeidentifyTemplateId;

            string projectId = _fixture.ProjectId;
            string locationId = _fixture.LocationId;

            TemplateName templateName = _fixture.CreateTemplateName();
            string templateId = templateName.TemplateId;

            // Build the inspect template name.
            string inspectTemplateName = DeidentifyTemplateName
                .FormatProjectLocationDeidentifyTemplate(projectId, locationId, inspectTemplateId)
                .ToString();

            // Build the deidentify template name.
            string deidentifyTemplateName = DeidentifyTemplateName
                .FormatProjectLocationDeidentifyTemplate(
                    projectId,
                    locationId,
                    deidentifyTemplateId
                )
                .ToString();

            // Run the sample.
            Template template = _sample.CreateTemplateWithAdvancedSdp(
                projectId: projectId,
                locationId: locationId,
                templateId: templateId,
                inspectTemplateId: inspectTemplateId,
                deidentifyTemplateId: deidentifyTemplateId
            );

            // Verify that template was created successfully.
            Assert.NotNull(template);
            Assert.Contains(templateId, template.Name);

            // Verify that created template has the expected filter configuration.
            Assert.NotNull(template.FilterConfig);
            Assert.NotNull(template.FilterConfig.SdpSettings);
            Assert.NotNull(template.FilterConfig.SdpSettings.AdvancedConfig);

            // Verify the advanced SDP configuration of the created template.
            var advancedConfig = template.FilterConfig.SdpSettings.AdvancedConfig;
            Assert.Equal(inspectTemplateName, advancedConfig.InspectTemplate);
            Assert.Equal(deidentifyTemplateName, advancedConfig.DeidentifyTemplate);

            _fixture.RegisterTemplateForCleanup(templateName);
        }
    }
}
