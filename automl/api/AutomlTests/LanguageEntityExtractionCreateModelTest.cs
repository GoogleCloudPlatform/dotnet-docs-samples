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
    public class LanguageEntityExtractionCreateModelTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _datasetId;
        public LanguageEntityExtractionCreateModelTest(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _datasetId = "TRL0000000000000000000";
        }

        [Fact]
        public void TestLanguageEntityExtractionCreateModel()
        {
            // As entity extraction does not let you cancel model creation, instead try to create a model
            // from a nonexistent dataset, but other elements of the request were valid.
            try
            {
                // Create a random dataset name with a length of 32 characters (max allowed by AutoML)
                // To prevent name collisions when running tests in multiple java versions at once.
                // AutoML doesn't allow "-", but accepts "_"
                string modelName = "test_language_entity_model_" + TestUtil.RandomName();
                modelName = modelName.Substring(0, 32);

                ConsoleOutput output = _fixture.SampleRunner.Run("create_model_language_entity_extraction", _fixture.ProjectId, modelName, _datasetId);
                Assert.Contains("Dataset does not exist", output.Stdout);
            }
            catch (Exception ex) when (ex is ThreadInterruptedException ||
                 ex is IOException || ex is RpcException || ex is AggregateException)
            {
                Assert.Contains("ID was set to a value not ", ex.Message);
            }
        }
    }
}
