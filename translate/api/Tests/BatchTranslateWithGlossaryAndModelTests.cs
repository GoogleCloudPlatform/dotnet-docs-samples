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

using System;
using Xunit;
using Google.Cloud.Storage.V1;
using CommandLine;

namespace GoogleCloudSamples
{
    public class BatchTranslateWithGlossaryAndModelTests : IDisposable
    {
        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private readonly string _modelId = "TRL8772189639420149760";
        private readonly string _glossaryInputUri = "gs://cloud-samples-data/translation/glossary_ja.csv";
        private readonly string _inputUri = "gs://cloud-samples-data/translation/text_with_custom_model_and_glossary.txt";
        private readonly string _bucketName;
        private readonly string _glossaryId;
        private readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = TranslateV3Samples.Main
        };

        // Setup
        public BatchTranslateWithGlossaryAndModelTests()
        {
            // Create temp bucket
            using (var storageClient = StorageClient.Create())
            {
                _bucketName = "translate-v3-" + TestUtil.RandomName();
                storageClient.CreateBucket(_projectId, _bucketName);
            }

            // Create temp glossary
            _glossaryId = "must-start-with-letters" + TestUtil.RandomName();
            TranslateV3CreateGlossary.CreateGlossarySample(_projectId, _glossaryId, _glossaryInputUri);
        }

        public void Dispose()
        {
            using (var storageClient = StorageClient.Create())
            {
                storageClient.DeleteBucket(_bucketName,
                new DeleteBucketOptions
                {
                    DeleteObjects = true
                });
            }

            // Clean up glossary
            TranslateV3DeleteGlossary.DeleteGlossarySample(_projectId, _glossaryId);
        }

        [Fact]
        public void BatchTranslateTextWithGlossaryAndModelTest()
        {
            string outputUri =
                string.Format("gs://{0}/translation/BATCH_TRANSLATION_OUTPUT/", _bucketName);

            var output = _sample.Run("batchTranslateTextWithGlossaryAndModel",
                "--project_id=" + _projectId,
                "--location=us-central1",
                "--source_language=en",
                "--target_language=ja",
                "--glossary_id=" + _glossaryId,
                "--model_id=" + _modelId,
                "--output_uri=" + outputUri,
                "--input_uri=" + _inputUri);
            Assert.Contains("Total Characters: 25", output.Stdout);
        }
    }
}
