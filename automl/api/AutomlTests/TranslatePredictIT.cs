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
    [Collection(nameof(AutoMLFixture))]
    public class TranslatePredictIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId = "TRL8772189639420149760";
        private readonly string _filePath = "./resources/input.txt";
        public TranslatePredictIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestPredictTranslation()
        {
            // Act
            ConsoleOutput output = _fixture.SampleRunner.Run("translate_predict",
_fixture.ProjectId, _modelId, _filePath);

            // Assert
            Assert.Contains("Translated Content:", output.Stdout);
        }
    }
}
