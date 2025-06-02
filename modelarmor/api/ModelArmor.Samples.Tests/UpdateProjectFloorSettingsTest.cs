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
using System.IO;
using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class UpdateProjectFloorSettingsTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly UpdateProjectFloorSettings _sample;
        private readonly ITestOutputHelper _output;

        public UpdateProjectFloorSettingsTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _sample = new UpdateProjectFloorSettings();
            _output = output;
        }

        [Fact]
        public void UpdateProjectFloorSettingTest()
        {
            // Get project ID from environment variable
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            // Skip test if project ID is not set
            if (string.IsNullOrEmpty(projectId))
            {
                _output.WriteLine("Skipping test: GOOGLE_PROJECT_ID environment variable not set");
                return;
            }

            // Run the sample
            FloorSetting response = _sample.UpdateProjectFloorSetting(projectId);

            // Verify response
            Assert.NotNull(response);
            Assert.Equal($"projects/{projectId}/locations/global/floorSetting", response.Name);
            Assert.True(response.EnableFloorSettingEnforcement);
            Assert.NotNull(response.FilterConfig);
            Assert.NotNull(response.FilterConfig.RaiSettings);
            Assert.Contains(
                response.FilterConfig.RaiSettings.RaiFilters,
                filter =>
                    filter.FilterType == RaiFilterType.HateSpeech
                    && filter.ConfidenceLevel == DetectionConfidenceLevel.High
            );
        }
    }
}
