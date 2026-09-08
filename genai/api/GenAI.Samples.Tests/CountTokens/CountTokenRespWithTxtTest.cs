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

using Google.GenAI.Types;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(GenAIFixture))]
public class CountTokenRespWithTxtTest
{
    private readonly GenAIFixture _fixture;
    private readonly CountTokenRespWithTxt _sample;

    public CountTokenRespWithTxtTest(GenAIFixture fixture)
    {
        _fixture = fixture;
        _sample = new CountTokenRespWithTxt();
    }

    [Fact]
    public async Task TestCountTokenRespWithTxt()
    {
        GenerateContentResponseUsageMetadata usageMetadata = await _sample.CountTokens(_fixture.ProjectId);
        Assert.NotNull(usageMetadata);
    }
}
