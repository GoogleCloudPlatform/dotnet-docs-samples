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
using Google.Api.Gax.ResourceNames;
using Google.Cloud.ModelArmor.V1;
using Xunit;
using Xunit.Abstractions;

namespace ModelArmor.Samples.Tests
{
    public class ScanPdfFileTests : IClassFixture<ModelArmorFixture>
    {
        private readonly ModelArmorFixture _fixture;
        private readonly ScanPdfFileSample _sample;
        private readonly ITestOutputHelper _output;
        private readonly string _testPdfPath;

        public ScanPdfFileTests(ModelArmorFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _sample = new ScanPdfFileSample();
            _output = output;

            // Create a path to a test PDF file
            string currentDirectory = Directory.GetCurrentDirectory();
            string rootDirectory = Directory.GetCurrentDirectory();
            int binIndex = rootDirectory.IndexOf("bin");
            string projectDirectory = rootDirectory.Substring(0, binIndex);
            _testPdfPath = Path.Combine(projectDirectory, "test_sample.pdf");

            // For testing purposes, we'll create a simple PDF file if it doesn't exist
            if (!File.Exists(_testPdfPath))
            {
                string content = "<pdf>This is a test PDF file.</pdf>";
                File.WriteAllText(_testPdfPath, content);
            }
        }

        [Fact]
        public void ScanPdfFileWithBaseTemplate()
        {
            // Arrange
            Template template = _fixture.CreateBaseTemplate();
            string templateId = TemplateName.Parse(template.Name).TemplateId;

            // Act
            SanitizeUserPromptResponse response = _sample.ScanPdfFile(
                _fixture.ProjectId,
                _fixture.LocationId,
                templateId,
                _testPdfPath
            );

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.SanitizationResult);
            Assert.Equal(InvocationResult.Success, response.SanitizationResult.InvocationResult);
        }

        [Fact]
        public void ScanPdfFileWithMaliciousContentTemplate()
        {
            // Arrange
            Template template = _fixture.CreateTemplateWithMaliciousUri();
            string templateId = TemplateName.Parse(template.Name).TemplateId;

            // Act
            SanitizeUserPromptResponse response = _sample.ScanPdfFile(
                _fixture.ProjectId,
                _fixture.LocationId,
                templateId,
                _testPdfPath
            );

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.SanitizationResult);
            Assert.Equal(InvocationResult.Success, response.SanitizationResult.InvocationResult);

            // Check for malicious URI filter results if applicable
            if (response.SanitizationResult.FilterResults.ContainsKey("malicious_uris"))
            {
                var filterResult = response.SanitizationResult.FilterResults["malicious_uris"];
                Assert.NotNull(filterResult.MaliciousUriFilterResult);
            }
        }

        [Fact]
        public void ScanPdfFileWithSdpTemplate()
        {
            // Arrange
            Template template = _fixture.CreateBasicSdpTemplate();
            string templateId = TemplateName.Parse(template.Name).TemplateId;

            // Act
            SanitizeUserPromptResponse response = _sample.ScanPdfFile(
                _fixture.ProjectId,
                _fixture.LocationId,
                templateId,
                _testPdfPath
            );

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.SanitizationResult);
            Assert.Equal(InvocationResult.Success, response.SanitizationResult.InvocationResult);

            // Check for SDP filter results if applicable
            if (response.SanitizationResult.FilterResults.ContainsKey("sdp"))
            {
                var sdpResult = response.SanitizationResult.FilterResults["sdp"];
                Assert.NotNull(sdpResult.SdpFilterResult);
            }
        }

        [Fact]
        public void ScanPdfFileWithPiAndJailbreakTemplate()
        {
            // Arrange
            Template template = _fixture.CreateTemplateWithPiAndJailbreak();
            string templateId = TemplateName.Parse(template.Name).TemplateId;

            // Act
            SanitizeUserPromptResponse response = _sample.ScanPdfFile(
                _fixture.ProjectId,
                _fixture.LocationId,
                templateId,
                _testPdfPath
            );

            // Assert
            Assert.NotNull(response);
            Assert.NotNull(response.SanitizationResult);
            Assert.Equal(InvocationResult.Success, response.SanitizationResult.InvocationResult);

            // Check for PI and Jailbreak filter results if applicable
            if (response.SanitizationResult.FilterResults.ContainsKey("pi_and_jailbreak"))
            {
                var filterResult = response.SanitizationResult.FilterResults["pi_and_jailbreak"];
                Assert.NotNull(filterResult.PiAndJailbreakFilterResult);
            }
        }

        [Fact]
        public void ScanPdfFileWithInvalidPath()
        {
            // Arrange
            Template template = _fixture.CreateBaseTemplate();
            string templateId = TemplateName.Parse(template.Name).TemplateId;
            string invalidPath = Path.Combine(Path.GetTempPath(), "non_existent_file.pdf");

            // Act & Assert
            var exception = Assert.Throws<FileNotFoundException>(() =>
                _sample.ScanPdfFile(
                    _fixture.ProjectId,
                    _fixture.LocationId,
                    templateId,
                    invalidPath
                )
            );

            Assert.Contains(invalidPath, exception.Message);
        }
    }
}
