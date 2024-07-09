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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(AIPlatformFixture))]
public class ControlledGenerationTest
{
    private readonly AIPlatformFixture _fixture;
    private readonly ControlledGeneration _sample;

    public ControlledGenerationTest(AIPlatformFixture fixture)
    {
        _fixture = fixture;
        _sample = new ControlledGeneration();
    }

    [Fact]
    public async Task TestGenerateContentWithResponseMimeType()
    {
        var response = await _sample.GenerateContentWithResponseMimeType(_fixture.ProjectId);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task TestGenerateContentWithResponseSchema()
    {
        var response = await _sample.GenerateContentWithResponseSchema(_fixture.ProjectId);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task TestGenerateContentWithResponseSchema2()
    {
        var response = await _sample.GenerateContentWithResponseSchema2(_fixture.ProjectId);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task TestGenerateContentWithResponseSchema3()
    {
        var response = await _sample.GenerateContentWithResponseSchema3(_fixture.ProjectId);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task TestGenerateContentWithResponseSchema4()
    {
        var response = await _sample.GenerateContentWithResponseSchema4(_fixture.ProjectId);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task TestGenerateContentWithResponseSchema6()
    {
        var response = await _sample.GenerateContentWithResponseSchema6(_fixture.ProjectId);
        Assert.NotNull(response);
    }
}
