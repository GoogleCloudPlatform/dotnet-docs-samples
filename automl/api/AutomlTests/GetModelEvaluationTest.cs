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

using Grpc.Core;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class GetModelEvaluationTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        private string _modelEvaluationId;
        public GetModelEvaluationTest(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TEN623594616762925056";
        }

        [Fact]
        public void TestGetModelEvaluation()
        {
            // List Model Evaluations
            ConsoleOutput outputListModelEval = _fixture.SampleRunner.Run("list_model_evaluations", _fixture.ProjectId,
                _modelId);

            // TODO: simplify and find easier way to do this!
            string[] fullListModels = Regex.Split(outputListModelEval.Stdout, @"\b(modelEvaluations/)\b");
            // capture first match of model eval id 
            _modelEvaluationId = fullListModels.Length > 2 ? fullListModels[2].Split("\n")[0] : "1277055869634302881";

            Assert.Contains("Model Evaluation Name:", outputListModelEval.Stdout);
        }
    }
}
