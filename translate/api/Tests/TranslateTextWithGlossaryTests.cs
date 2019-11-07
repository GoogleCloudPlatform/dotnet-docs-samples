// Copyright 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(TranslateFixture))]
    public class TranslateTextWithGlossaryTests
    {
        private readonly TranslateFixture _fixture;

        public TranslateTextWithGlossaryTests(TranslateFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TranslateTextWithGlossaryTest()
        {
            var output = _fixture.SampleRunner.Run("translateTextWithGlossary",
                "--project_id=" + _fixture.ProjectId,
                "--text=account",
                "--target_language=ja",
                "--glossary_id=" + _fixture.GlossaryId);
            Assert.True(output.Stdout.Contains("\u30A2\u30AB\u30A6\u30F3\u30C8") || output.Stdout.Contains("\u53E3\u5EA7"));
        }
    }
}



