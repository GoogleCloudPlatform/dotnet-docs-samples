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
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
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
        public void SanitizeModelResponseWithRaiTemplates()
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
            Assert.Equal(
                FilterMatchState.NoMatchFound,
                response.SanitizationResult.FilterMatchState
            );
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

        [Fact]
        public void SanitizeModelResponseWithMaliciousUrlTemplate()
        {
            // Arrange
            string unsafeResponse =
                "You can use this to make a cake: https://testsafebrowsing.appspot.com/s/malware.html";
            Template template = _fixture.CreateTemplateWithMaliciousUri();
            string templateId = TemplateName.Parse(template.Name).TemplateId;

            // Act
            SanitizeModelResponseResponse response = _sample.SanitizeModelResponse(
                _fixture.ProjectId,
                _fixture.LocationId,
                templateId,
                unsafeResponse
            );

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.SanitizationResult);
            Assert.Equal(FilterMatchState.MatchFound, response.SanitizationResult.FilterMatchState);
            Assert.Equal(InvocationResult.Success, response.SanitizationResult.InvocationResult);

            if (response.SanitizationResult.FilterResults.ContainsKey("malicious_uris"))
            {
                var filterResult = response.SanitizationResult.FilterResults["malicious_uris"];

                // Check if the filter result has malicious URI filter result
                if (filterResult.MaliciousUriFilterResult != null)
                {
                    // Assert that the match state is MATCH_FOUND
                    Assert.Equal(
                        FilterMatchState.MatchFound,
                        filterResult.MaliciousUriFilterResult.MatchState
                    );

                    // You can also verify the matched items if needed
                    Assert.NotEmpty(filterResult.MaliciousUriFilterResult.MaliciousUriMatchedItems);

                    // Check the specific URI that was matched
                    var matchedItem = filterResult
                        .MaliciousUriFilterResult
                        .MaliciousUriMatchedItems[0];
                    Assert.Equal(
                        "https://testsafebrowsing.appspot.com/s/malware.html",
                        matchedItem.Uri
                    );

                    // Check the location of the match
                    Assert.NotEmpty(matchedItem.Locations);
                    var location = matchedItem.Locations[0];
                    Assert.True(location.Start >= 0);
                    Assert.True(location.End > location.Start);
                }
            }
        }

        [Fact]
        public void testSanitizeModelResponseWithBasicSdpTemplate()
        {
            // Arrange
            string modelResponse =
                "For following email 1l6Y2@example.com found following"
                + " associated phone number: 954-321-7890 and this ITIN: 988-86-1234";
            Template template = _fixture.CreateBasicSdpTemplate();
            string templateId = TemplateName.Parse(template.Name).TemplateId;

            // Act
            SanitizeModelResponseResponse response = _sample.SanitizeModelResponse(
                _fixture.ProjectId,
                _fixture.LocationId,
                templateId,
                modelResponse
            );

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.SanitizationResult);
            Assert.Equal(FilterMatchState.MatchFound, response.SanitizationResult.FilterMatchState);
            Assert.Equal(InvocationResult.Success, response.SanitizationResult.InvocationResult);

            // Verify SDP filter results
            if (response.SanitizationResult.FilterResults.ContainsKey("sdp"))
            {
                var filterResultsMap = response.SanitizationResult.FilterResults;
                foreach (var entry in filterResultsMap)
                {
                    string filterName = entry.Key;
                    var filterResult = entry.Value;

                    if (filterResult.SdpFilterResult != null)
                    {
                        if (filterResult.SdpFilterResult.InspectResult != null)
                        {
                            Assert.Equal(
                                FilterMatchState.MatchFound,
                                filterResult.SdpFilterResult.InspectResult.MatchState
                            );

                            var findings = filterResult.SdpFilterResult.InspectResult.Findings;
                            foreach (var finding in findings)
                            {
                                Assert.Equal(
                                    "US_INDIVIDUAL_TAXPAYER_IDENTIFICATION_NUMBER",
                                    finding.InfoType
                                );
                            }
                        }
                    }
                }
            }
        }

        [Fact]
        public void testSanitizeModelResponseWithAdvancedSdpTemplate()
        {
            // Arrange
            string modelResponse =
                "For following email 1l6Y2@example.com found following"
                + " associated phone number: 954-321-7890 and this ITIN: 988-86-1234";
            Template template = _fixture.CreateBasicSdpTemplate();
            string templateId = TemplateName.Parse(template.Name).TemplateId;

            // Act
            SanitizeModelResponseResponse response = _sample.SanitizeModelResponse(
                _fixture.ProjectId,
                _fixture.LocationId,
                templateId,
                modelResponse
            );

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.SanitizationResult);
            Assert.Equal(FilterMatchState.MatchFound, response.SanitizationResult.FilterMatchState);
            Assert.Equal(InvocationResult.Success, response.SanitizationResult.InvocationResult);

            // Verify SDP filter results
            if (response.SanitizationResult.FilterResults.ContainsKey("sdp"))
            {
                var filterResultsMap = response.SanitizationResult.FilterResults;
                foreach (var entry in filterResultsMap)
                {
                    string filterName = entry.Key;
                    var filterResult = entry.Value;

                    if (filterResult.SdpFilterResult != null)
                    {
                        // Verify Inspect Result
                        if (filterResult.SdpFilterResult.InspectResult != null)
                        {
                            Assert.Equal(
                                FilterMatchState.MatchFound,
                                filterResult.SdpFilterResult.InspectResult.MatchState
                            );

                            var findings = filterResult.SdpFilterResult.InspectResult.Findings;
                            foreach (var finding in findings)
                            {
                                Assert.Equal(
                                    "US_INDIVIDUAL_TAXPAYER_IDENTIFICATION_NUMBER",
                                    finding.InfoType
                                );
                            }
                        }

                        // Verify De-identified Result
                        if (filterResult.SdpFilterResult.DeidentifyResult != null)
                        {
                            Assert.Equal(
                                FilterMatchState.MatchFound,
                                filterResult.SdpFilterResult.DeidentifyResult.MatchState
                            );
                            Assert.NotNull(filterResult.SdpFilterResult.DeidentifyResult.Data.Text);
                        }
                    }
                }
            }
        }
    }
}
