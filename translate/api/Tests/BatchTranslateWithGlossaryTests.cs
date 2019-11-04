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

namespace GoogleCloudSamples
{
    public class BatchTranslateWithGlossaryTests : IDisposable
    {
        private readonly string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private readonly string _glossaryInputUri = "gs://cloud-samples-data/translation/glossary_ja.csv";
        private readonly string _inputUri = "gs://cloud-samples-data/translation/text_with_glossary.txt";
        private readonly string _bucketName;
        private readonly string _glossaryId;

        private readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = TranslateV3Samples.Main
        };

        // Setup
        public BatchTranslateWithGlossaryTests()
        {
            // Create temp bucket
            using (var storageClient = StorageClient.Create())
            {
                _bucketName = "translate-v3-" + TestUtil.RandomName();
                storageClient.CreateBucket(_projectId, _bucketName);
            }

            // Create temp glossary
            _glossaryId = "must-start-with-letters" + TestUtil.RandomName();
            CreateGlossary.CreateGlossarySample(_projectId, _glossaryId, _glossaryInputUri);
        }

        public void Dispose()
        {
            using (var storageClient = StorageClient.Create())
            {
                storageClient.DeleteBucket(_bucketName,
                new DeleteBucketOptions
                {
                    DeleteObjects = true
                }); ;
            }

            // Clean up glossary
            DeleteGlossary.DeleteGlossarySample(_projectId, _glossaryId);
        }

        [Fact]
        public void BatchTranslateTextWithGlossaryTest()
        {
            string outputUri =
                string.Format("gs://{0}/translation/BATCH_TRANSLATION_OUTPUT/", _bucketName);

            var output = _sample.Run("batchTranslateTextWithGlossary",
                "--project_id=" + _projectId,
                "--location=us-central1",
                "--source_language=en",
                "--target_language=ja",
                "--glossary_id=" + _glossaryId,
                "--output_uri=" + outputUri,
                "--input_uri=" + _inputUri);

            Assert.Contains("Total Characters: 9", output.Stdout);
        }
    }
}
