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

public class SanitizeUserPromptTests : IClassFixture<ModelArmorFixture>
{
    private readonly ModelArmorFixture _fixture;
    private readonly SanitizeUserPromptSample _sample;
    private readonly ITestOutputHelper _output;

    public SanitizeUserPromptTests(ModelArmorFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _sample = new SanitizeUserPromptSample();
        _output = output;
    }

    [Fact]
    public void SanitizeUserPrompt()
    {
        // Arrange
        string userPrompt = "How to make cheesecake without oven at home?";
        Template template = _fixture.CreateBaseTemplate();
        string templateId = TemplateName.Parse(template.Name).TemplateId;

        // Act
        SanitizeUserPromptResponse response = _sample.SanitizeUserPrompt(
            _fixture.ProjectId,
            _fixture.LocationId,
            templateId,
            userPrompt
        );

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.SanitizationResult);
        Assert.Equal(FilterMatchState.NoMatchFound, response.SanitizationResult.FilterMatchState);

        // Check RAI filter results if available
        if (response.SanitizationResult.FilterResults.ContainsKey("rai"))
        {
            var filterResultsMap = response.SanitizationResult.FilterResults;
            foreach (var entry in filterResultsMap)
            {
                string filterName = entry.Key;
                var filterResult = entry.Value;

                if (filterResult.RaiFilterResult != null)
                {
                    var raiFilterResult = filterResult.RaiFilterResult;
                    Assert.Equal(FilterMatchState.NoMatchFound, raiFilterResult.MatchState);

                    // Check each RAI filter type result
                    foreach (var typeEntry in raiFilterResult.RaiFilterTypeResults)
                    {
                        string raiFilterType = typeEntry.Key;
                        var raiFilterTypeResult = typeEntry.Value;
                        Assert.Equal(FilterMatchState.NoMatchFound, raiFilterTypeResult.MatchState);
                    }
                }
            }
        }
    }
}
