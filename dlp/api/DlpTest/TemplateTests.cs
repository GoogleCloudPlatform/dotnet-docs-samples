// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using GoogleCloudSamples;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DlpTest
{
    /// <summary>
    /// Tests for DLP Template APIs
    /// </summary>
    public class TemplateTests : IClassFixture<DlpTestFixture>
    {
        private readonly DlpTestFixture _testSettings;

        public TemplateTests(DlpTestFixture fixture)
        {
            _testSettings = fixture;
        }

        [Fact]
        public void TestTemplates()
        {
            // Creation
            string name = $"my-inspect-template-{TestUtil.RandomName()}";
            string displayName = $"My display name {Guid.NewGuid()}";
            string description = $"My description {Guid.NewGuid()}";
            string fullName = $"projects/{_testSettings.ProjectId}/inspectTemplates/{name}";

            ConsoleOutput output = _testSettings.CommandLineRunner.Run(
                "createInspectTemplate",
                _testSettings.ProjectId,
                name,
                displayName,
                description);

            Assert.Contains($"Successfully created template {fullName}", output.Stdout);

            // List
            output = _testSettings.CommandLineRunner.Run("listTemplates", _testSettings.ProjectId);
            Assert.Contains($"Template {fullName}", output.Stdout);
            Assert.Contains($"Display Name: {displayName}", output.Stdout);
            Assert.Contains($"Description: {description}", output.Stdout);

            // Deletion
            output = _testSettings.CommandLineRunner.Run("deleteTemplate", _testSettings.ProjectId, fullName);
            Assert.Contains($"Successfully deleted template {fullName}", output.Stdout);
        }
    }
}
