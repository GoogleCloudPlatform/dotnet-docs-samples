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
    public class ImportDatasetTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _datasetId;
        public ImportDatasetTest(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _datasetId = "TEN3278932790028009472";
        }

        [Fact]
        public void TestImportDataset()
        {
            // set up
            ConsoleOutput output = _fixture.SampleRunner.Run("create_dataset_language_entity_extraction",
            _fixture.ProjectId, _fixture.DatasetName);

            string datasetId = output.Stdout.Split("\n")[1].Split()[2];


            // import data
            string bucket = $"gs://{ _fixture.ProjectId}-lcm";

            string data = bucket + "/entity_extraction/dataset.csv";
            output = _fixture.SampleRunner.Run("import_dataset",
            _fixture.ProjectId, datasetId, data);

            Assert.Contains("Data imported.", output.Stdout);

            // tear down
            output = _fixture.SampleRunner.Run("delete_dataset",
            _fixture.ProjectId, datasetId);

            Assert.Contains("Dataset deleted", output.Stdout);
        }
    }
}
