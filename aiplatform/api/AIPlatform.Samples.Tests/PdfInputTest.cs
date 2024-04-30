/*
 * Copyright 2024 Google LLC
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
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(AIPlatformFixture))]
public class PdfInputTest
{
    private readonly AIPlatformFixture _fixture;
    private readonly PdfInput _sample;

    public PdfInputTest(AIPlatformFixture fixture)
    {
        _fixture = fixture;
        _sample = new PdfInput();
    }

    [Fact]
    public async Task TestSummarizePdf()
    {
        var response = await _sample.SummarizePdf(_fixture.ProjectId);
        Assert.Contains("gemini", response, StringComparison.OrdinalIgnoreCase);
    }
}
