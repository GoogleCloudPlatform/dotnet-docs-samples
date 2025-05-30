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
        string projectId = _fixture.ProjectId;
        string locationId = _fixture.LocationId;

        TemplateName templateName = _fixture.CreateTemplateName();
        _fixture.RegisterTemplateForCleanup(templateName);
        string templateId = templateName.TemplateId;

        string inspectTemplateName = _fixture.CreateInspectTemplate();
        _fixture.RegisterDlpTemplateForCleanup(inspectTemplateName);

        string deidentifyTemplateName = _fixture.CreateDeidentifyTemplate();
        _fixture.RegisterDlpTemplateForCleanup(deidentifyTemplateName);

        // Run the sample.
        Template template = _sample.CreateTemplateWithAdvancedSdp(
            projectId: projectId,
            locationId: locationId,
            templateId: templateId,
            inspectTemplateName: inspectTemplateName,
            deidentifyTemplateName: deidentifyTemplateName
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
    }
}
