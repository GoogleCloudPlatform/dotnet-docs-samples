/*
 * Copyright 2023 Google LLC
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

using Xunit;

[Collection(nameof(DocumentAIFixture))]
public class QuickstartTest
{
    private readonly DocumentAIFixture _fixture;
    private readonly QuickstartSample _sample;

    public QuickstartTest(DocumentAIFixture fixture)
    {
        _fixture = fixture;
        _sample = new QuickstartSample();
    }

    [Fact]
    public void Runs()
    {
        // Run the sample code.
        var document = _sample.Quickstart(projectId: _fixture.ProjectId, locationId: _fixture.LocationId, processorId: _fixture.ProcessorId, localPath: _fixture.LocalPath, mimeType: _fixture.MimeType);
        Assert.Contains("Invoice", document.Text);
    }
}
