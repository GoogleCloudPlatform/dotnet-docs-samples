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
using System.Threading;
using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class UndeployModelTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId;
        public UndeployModelTest(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _modelId = "TEN0000000000000000000";
        }

        [Fact]
        public void TestUndeployModel()
        {
            // As model deployment can take a long time, instead try to deploy a
            // nonexistent model and confirm that the model was not found, but other
            // elements of the request were valid.
            try
            {
                ConsoleOutput output = _fixture.SampleRunner.Run("undeploy_model", _fixture.ProjectId, _modelId);
                Assert.Contains("The model does not exist", output.Stdout);
            }
            catch (Exception ex) when (ex is ThreadInterruptedException ||
                 ex is IOException || ex is RpcException || ex is AggregateException)
            {
                Assert.Contains("The model does not exist", ex.Message);
            }
        }
    }
}
