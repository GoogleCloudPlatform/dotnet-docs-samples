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
public class MultimodalAllInputTest
{
    private readonly AIPlatformFixture _fixture;
    private readonly MultimodalAllInput _sample;

    public MultimodalAllInputTest(AIPlatformFixture fixture)
    {
        _fixture = fixture;
        _sample = new MultimodalAllInput();
    }

    [Fact]
    public async Task TestAnswerFromMultimodalInput()
    {
        var response = await _sample.AnswerFromMultimodalInput(_fixture.ProjectId);
        Assert.Contains("0:48", response, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("blind", response, StringComparison.OrdinalIgnoreCase);
    }
}
