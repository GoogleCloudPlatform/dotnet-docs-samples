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

public class SanitizeModelResponseTests : IClassFixture<ModelArmorFixture>
{
    private readonly ModelArmorFixture _fixture;
    private readonly SanitizeModelResponseSample _sample;
    private readonly ITestOutputHelper _output;

    public SanitizeModelResponseTests(ModelArmorFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _sample = new SanitizeModelResponseSample();
        _output = output;
    }

    [Fact]
    public void SanitizeModelResponse()
    {
        // Arrange
        string safeResponse =
            "To make cheesecake without oven, you'll need to follow these steps...";
        Template template = _fixture.CreateBaseTemplate();
        string templateId = TemplateName.Parse(template.Name).TemplateId;

        // Act
        SanitizeModelResponseResponse response = _sample.SanitizeModelResponse(
            _fixture.ProjectId,
            _fixture.LocationId,
            templateId,
            safeResponse
        );

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.SanitizationResult);
        Assert.Equal(FilterMatchState.NoMatchFound, response.SanitizationResult.FilterMatchState);
        Assert.Equal(InvocationResult.Success, response.SanitizationResult.InvocationResult);

        // Verify Rai filter results
        if (response.SanitizationResult.FilterResults.ContainsKey("rai"))
        {
            var filterResult = response.SanitizationResult.FilterResults["rai"];

            // Check if the filter result has RAI filter result
            if (filterResult.RaiFilterResult != null)
            {
                var raiFilterResult = filterResult.RaiFilterResult;
                Assert.Equal(FilterMatchState.NoMatchFound, raiFilterResult.MatchState);

                // Iterate through all RAI filter type results
                foreach (var entry in raiFilterResult.RaiFilterTypeResults)
                {
                    string raiFilterType = entry.Key;
                    Assert.Equal(FilterMatchState.NoMatchFound, entry.Value.MatchState);
                }
            }
        }
    }
}
