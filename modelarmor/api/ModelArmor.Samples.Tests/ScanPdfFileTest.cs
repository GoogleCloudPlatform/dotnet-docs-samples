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
using System.IO;
using Xunit;
using Xunit.Abstractions;

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

        string rootDirectory = Directory.GetCurrentDirectory();
        int binIndex = rootDirectory.IndexOf("bin");
        string projectDirectory = rootDirectory.Substring(0, binIndex);
        _testPdfPath = Path.Combine(projectDirectory, "test_sample.pdf");
    }

    [Fact]
    public void ScanPdfFile()
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
}
