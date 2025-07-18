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
using ModelArmor.Samples;
using Xunit;
using Xunit.Abstractions;

public class SanitizeModelResponseWithUserPromptTests : IClassFixture<ModelArmorFixture>
{
    private readonly ModelArmorFixture _fixture;
    private readonly SanitizeModelResponseWithUserPromptSample _sample;
    private readonly ITestOutputHelper _output;

    public SanitizeModelResponseWithUserPromptTests(
        ModelArmorFixture fixture,
        ITestOutputHelper output
    )
    {
        _fixture = fixture;
        _sample = new SanitizeModelResponseWithUserPromptSample();
        _output = output;
    }

    [Fact]
    public void SanitizeModelResponseWithUserPrompt()
    {
        // Arrange
        Template template = _fixture.CreateBaseTemplate();
        string templateId = TemplateName.Parse(template.Name).TemplateId;
        string userPrompt =
            "How can i make my email address test@dot.com make available to public for feedback";
        string modelResponse =
            "You can make support email such as contact@email.com for getting feedback from your customer";

        // Act
        SanitizeModelResponseResponse sanitizedResponse =
            _sample.SanitizeModelResponseWithUserPrompt(
                _fixture.ProjectId,
                _fixture.LocationId,
                templateId,
                userPrompt,
                modelResponse
            );

        // Assert
        Assert.NotNull(sanitizedResponse);
        Assert.NotNull(sanitizedResponse.SanitizationResult);
        Assert.Equal(
            FilterMatchState.NoMatchFound,
            sanitizedResponse.SanitizationResult.FilterMatchState
        );

        // Check CSAM filter result
        if (sanitizedResponse.SanitizationResult.FilterResults.ContainsKey("csam"))
        {
            var csamFilterResult = sanitizedResponse.SanitizationResult.FilterResults["csam"];
            Assert.Equal(
                FilterMatchState.NoMatchFound,
                csamFilterResult.CsamFilterFilterResult.MatchState
            );
        }
    }
}
