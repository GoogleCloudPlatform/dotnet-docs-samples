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

public class GetTemplateTests : IClassFixture<ModelArmorFixture>
{
    private readonly ModelArmorFixture _fixture;
    private readonly CreateTemplateSample _create_template_sample;
    private readonly GetTemplateSample _get_template_sample;
    private readonly (RaiFilterType, DetectionConfidenceLevel)[] expectedFilters =
    [
        (RaiFilterType.Dangerous, DetectionConfidenceLevel.High),
        (RaiFilterType.HateSpeech, DetectionConfidenceLevel.High),
        (RaiFilterType.Harassment, DetectionConfidenceLevel.MediumAndAbove),
        (RaiFilterType.SexuallyExplicit, DetectionConfidenceLevel.LowAndAbove),
    ];

    public GetTemplateTests(ModelArmorFixture fixture)
    {
        _fixture = fixture;
        _create_template_sample = new CreateTemplateSample();
        _get_template_sample = new GetTemplateSample();
    }

    [Fact]
    public void GetTemplateTest()
    {
        // Create a template for testing purpose.
        TemplateName templateName = _fixture.CreateTemplateName();
        _fixture.RegisterTemplateForCleanup(templateName);

        Template createdTemplate = _create_template_sample.CreateTemplate(
            projectId: _fixture.ProjectId,
            locationId: _fixture.LocationId,
            templateId: templateName.TemplateId
        );

        Template retrievedTemplate = _get_template_sample.GetTemplate(
            projectId: _fixture.ProjectId,
            locationId: _fixture.LocationId,
            templateId: templateName.TemplateId
        );

        var retrievedRaiFilters = retrievedTemplate.FilterConfig.RaiSettings.RaiFilters;

        foreach (var (type, confidence) in expectedFilters)
        {
            Assert.Contains(retrievedRaiFilters, f => f.FilterType == type);
            Assert.Contains(
                retrievedRaiFilters,
                f => f.FilterType == type && f.ConfidenceLevel == confidence
            );
        }
    }
}
