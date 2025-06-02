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
    public class GetOrganizationFloorSettingsTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly GetOrganizationFloorSettings _sample;
        private readonly ITestOutputHelper _output;

        public GetOrganizationFloorSettingsTests(
            ModelArmorFixture fixture,
            ITestOutputHelper output
        )
        {
            _fixture = fixture;
            _sample = new GetOrganizationFloorSettings();
            _output = output;
        }

        [Fact]
        public void GetOrganizationFloorSettingTest()
        {
            // Get organization ID from environment variable
            string organizationId = Environment.GetEnvironmentVariable("MA_ORG_ID");

            // Skip test if organization ID is not set
            if (string.IsNullOrEmpty(organizationId))
            {
                _output.WriteLine("Skipping test: MA_ORG_ID environment variable not set");
                return;
            }

            // Run the sample
            FloorSetting response = _sample.GetOrganizationFloorSetting(organizationId);

            // Verify response
            Assert.NotNull(response);
            Assert.Equal(
                $"organizations/{organizationId}/locations/global/floorSetting",
                response.Name
            );
        }
    }
}
