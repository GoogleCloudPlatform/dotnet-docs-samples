<<<<<<< HEAD
﻿// Copyright 2019 Google LLC
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
=======
﻿using Xunit;
>>>>>>> d42fde96... finished IT

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageSentimentAnalysisPredictIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public LanguageSentimentAnalysisPredictIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TST3802432266244718592";
        }

        [Fact]
        public void TestPredict()
        {
            string text = "Hopefully this Claritin kicks in soon";
            ConsoleOutput output = _fixture.SampleRunner.Run("language_entity_extraction_predict",
_fixture.ProjectId, _modelId, text);

            Assert.Contains("Predicted sentiment score:", output.Stdout);
        }
    }
}
