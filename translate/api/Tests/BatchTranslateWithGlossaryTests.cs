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
    [Collection(nameof(TranslateFixture))]
    public class BatchTranslateWithGlossaryTests
    {
        private readonly string _inputUri = "gs://cloud-samples-data/translation/text_with_glossary.txt";
        private readonly TranslateFixture _fixture;

        private readonly CommandLineRunner _sample = new CommandLineRunner()
        {
            VoidMain = TranslateV3Samples.Main
        };

        public BatchTranslateWithGlossaryTests(TranslateFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void BatchTranslateTextWithGlossaryTest()
        {
            string outputUri =
                string.Format("gs://{0}/translation/BATCH_TRANSLATE_GLOSSARY_OUTPUT/", _fixture._bucketName);

            var output = _sample.Run("batchTranslateTextWithGlossary",
                "--project_id=" + _fixture._projectId,
                "--location=us-central1",
                "--source_language=en",
                "--target_language=ja",
                "--glossary_id=" + _fixture._glossaryId,
                "--output_uri=" + outputUri,
                "--input_uri=" + _inputUri);

            Assert.Contains("Total Characters: 9", output.Stdout);
        }
    }
}
