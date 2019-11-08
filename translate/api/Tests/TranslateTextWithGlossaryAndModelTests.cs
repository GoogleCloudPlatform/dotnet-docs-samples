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
    public class TranslateTextWithGlossaryAndModelTests
    {
        private readonly TranslateFixture _fixture;

        public TranslateTextWithGlossaryAndModelTests(TranslateFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TranslateTextWithGlossaryAndModelTest()
        {
            var output = _fixture.SampleRunner.Run("translateTextWithGlossaryAndModel",
                "--project_id=" + _fixture.ProjectId,
                "--location=us-central1",
                "--text=That' il do it. deception",
                "--target_language=ja",
                "--glossary_id=" + _fixture.GlossaryId,
                "--model_id=" + _fixture.ModelId);
            Assert.True(output.Stdout.Contains("\u3084\u308B\u6B3A\u304F")
                || output.Stdout.Contains("\u305D\u308C\u3058\u3083\u3042")); // custom model
            Assert.Contains("\u6B3A\u304F", output.Stdout); //glossary
        }
    }
}


