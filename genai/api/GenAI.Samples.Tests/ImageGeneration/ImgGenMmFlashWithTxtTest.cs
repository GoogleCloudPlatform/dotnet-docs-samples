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

using System.IO;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(GenAIFixture))]
public class ImgGenMmFlashWithTxtTest
{
    private readonly GenAIFixture _fixture;
    private readonly ImgGenMmFlashWithTxt _sample;

    public ImgGenMmFlashWithTxtTest(GenAIFixture fixture)
    {
        _fixture = fixture;
        _sample = new ImgGenMmFlashWithTxt();
    }

    [Fact]
    public async Task TestImgGenMmFlashWithTxt()
    {
        FileInfo response = await _sample.GenerateContent(_fixture.ProjectId);
        Assert.NotNull(response);
        Assert.True(response.Length > 0);
    }
}
