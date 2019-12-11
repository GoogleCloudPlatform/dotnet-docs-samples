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
    public class GenericModelManagementTests
    {
        private readonly AutoMLFixture _fixture;
        private string _modelId;
        private string _modelEvaluationId;
        private string _operationId;
        public GenericModelManagementTests(AutoMLFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestOperationStatus()
        {
            // Act
            ConsoleOutput output = _fixture.SampleRunner.Run("list_operation_status",
_fixture.ProjectId);

            _operationId = "" + output.Stdout.Split("\n")[1].Split(":")[1].Trim();

            // Assert
            Assert.Contains("Operation details:", output.Stdout);

            // Act
            output = _fixture.SampleRunner.Run("get_operation_status",
                _operationId);

            // Assert 
            Assert.Contains("Operation details", output.Stdout);
            Assert.Contains(_operationId, output.Stdout);
        }

        [Fact]
        public void TestModelApi()
        {
            // List models
            ConsoleOutput output = _fixture.SampleRunner.Run("list_model",
          _fixture.ProjectId);
            Assert.Contains("Model id:", output.Stdout);

            _modelId = output.Stdout.Split("Model id: ")[1].Split("\n")[0];

            // Get Model
            output = _fixture.SampleRunner.Run("get_model", _fixture.ProjectId,
                _modelId);
            Assert.Contains($"Model id: {_modelId}", output.Stdout);

            // List Model Evaluations
            ConsoleOutput outputListModelEval = _fixture.SampleRunner.Run("list_model_evaluations", _fixture.ProjectId,
                _modelId);

            // TODO: simplify and find easier way to do this!
            string[] fullListModels = Regex.Split(outputListModelEval.Stdout, @"\b(modelEvaluations/)\b");
            // capture first match of model eval id 
            _modelEvaluationId = fullListModels.Length > 2 ? fullListModels[2].Split("\n")[0] : "1277055869634302881";

            Assert.Contains("Model Evaluation Name:", outputListModelEval.Stdout);

            // Get Model Evaluation
            output = _fixture.SampleRunner.Run("get_model_evaluation",
                _fixture.ProjectId,
                _modelId, _modelEvaluationId);
            Assert.Contains("Model Evaluation Name:", output.Stdout);
            Assert.Contains(_modelEvaluationId, output.Stdout);
        }

        [Fact]
        public void testDeleteModel()
        {
            // As model creation can take many hours, instead try to delete a
            // nonexistent model and confirm that the model was not found, but other
            // elements of the request were valid.
            try
            {
                _fixture.SampleRunner.Run("delete_model",
                                   _fixture.ProjectId,
                                   "TRL0000000000000000000");
            }
            catch (Exception ex) when (ex is ThreadInterruptedException ||
                   ex is IOException || ex is RpcException)
            {
                Assert.Contains("The model does not exist", ex.Message);
            }
        }
    }
}
