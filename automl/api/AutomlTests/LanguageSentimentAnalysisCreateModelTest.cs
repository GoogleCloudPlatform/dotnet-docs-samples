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

using Google.Cloud.AutoML.V1;
using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class LanguageSentimentAnalysisCreateModelTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _datasetId;
        private readonly string _operationId;
        public LanguageSentimentAnalysisCreateModelTest(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _datasetId = "TST6012617763841900544";
        }

        [Fact]
        public void TestCreateModel()
        {
            //TODO: FIX --> cannot cancel long running operation in teardown, not sure calling it from right client library.

            //// create dataset
            //ConsoleOutput output = _fixture.SampleRunner.Run("create_model_language_sent_analysis",
            //_fixture.ProjectId, _fixture.DatasetName, _datasetId);

            //Assert.Contains("Training started", output.Stdout);

            //_operationId = output.Stdout.Split("Training operation name: ")[1].Split("\n")[0];
            //// tear down

            //AutoMlClient client = AutoMlClient.Create();
            //    client.CreateModelOperationsClient.CancelOperation(_operationId);

        }
    }
}
