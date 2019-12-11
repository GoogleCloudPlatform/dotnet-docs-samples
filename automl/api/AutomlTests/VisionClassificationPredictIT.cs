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
    public class VisionClassificationPredictIT
    {
        private readonly AutoMLFixture _fixture;
        private readonly string _modelId = "ICN8261418109806575616";
        private readonly string _bucketId;
        public VisionClassificationPredictIT(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _bucketId = _fixture.ProjectId + "-vcm";
        }

        [Fact]
        public void TestVisionClassPredict()
        {
            string filePath = "resources/test.png";
            ConsoleOutput output = _fixture.SampleRunner.Run("vision_classification_predict",
_fixture.ProjectId, _modelId, filePath);

            Assert.Contains("Predicted class name:", output.Stdout);
        }

        [Fact]
        public void TestVisionBatchPredict()
        {
            string inputUri = $"gs://{_bucketId}/batch_predict_test.csv";
            string outputUri = $"gs://{_fixture.Bucket.Name}/TEST_BATCH_PREDICT/";

            // Act
            ConsoleOutput output = _fixture.SampleRunner.Run("vision_batch_predict",
_fixture.ProjectId, _modelId, inputUri, outputUri);

            // Assert
            Assert.Contains("Batch Prediction results saved to specified Cloud Storage bucket", output.Stdout);
        }
    }
}
