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
public class ToolsFuncDescWithTxtTest
{
    private readonly GenAIFixture _fixture;
    private readonly ToolsFuncDescWithTxt _sample;

    public ToolsFuncDescWithTxtTest(GenAIFixture fixture)
    {
        _fixture = fixture;
        _sample = new ToolsFuncDescWithTxt();
    }

    [Fact]
    public async Task TestToolsFuncDescWithTxt()
    {
        FunctionCall response = await _sample.GenerateContent(_fixture.ProjectId);
        Assert.NotNull(response);
        Assert.NotEmpty(response.Name);
        Assert.NotEmpty(response.Args);
    }
}
