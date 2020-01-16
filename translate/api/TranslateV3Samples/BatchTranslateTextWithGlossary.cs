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

// [START translate_v3_batch_translate_text_with_glossary]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class BatchTranslateTextWithGlossary
    {
        /// <summary>
        /// Translates a batch of texts on GCS and stores the result in a GCS location.
        /// Glossary is applied for translation.
        /// </summary>
        /// <param name="inputUri">The GCS path where input configuration is stored.</param>
        /// <param name="outputUri">The GCS path for translation output.</param>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="location">Region.</param>
        /// <param name="glossaryId">Required. Translation Glossary ID.</param>
        /// <param name="targetLanguage">Target language code.</param>
        /// <param name="sourceLanguage">Source language code.</param>
        public static void BatchTranslateTextWithGlossarySample(
            string inputUri = "gs://cloud-samples-data/text.txt",
            string outputUri = "gs://YOUR_BUCKET_ID/path_to_store_results/",
            string projectId = "[Google Cloud Project ID]",
            string location = "us-central1",
            string glossaryId = "[YOUR_GLOSSARY_ID]",
            string targetLanguage = "en",
            string sourceLanguage = "ja")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();

            string glossaryPath = $"projects/{projectId}/locations/{location}/glossaries/{glossaryId}";
            BatchTranslateTextRequest request = new BatchTranslateTextRequest
            {
                ParentAsLocationName = new LocationName(projectId, location),
                SourceLanguageCode = sourceLanguage,
                TargetLanguageCodes =
                {
                    // Target language code.
                    targetLanguage,
                },
                InputConfigs =
                {
                    new InputConfig
                    {
                        GcsSource = new GcsSource
                        {
                            InputUri = inputUri,
                        },
                        MimeType = "text/plain",
                    },
                },
                OutputConfig = new OutputConfig
                {
                    GcsDestination = new GcsDestination
                    {
                        OutputUriPrefix = outputUri,
                    },
                },
                Glossaries =
                {
                    {
                        targetLanguage, new TranslateTextGlossaryConfig
                        {
                            Glossary = glossaryPath,
                        }
                    },
                },
            };
            // Poll until the returned long-running operation is completed.
            BatchTranslateResponse response = translationServiceClient.BatchTranslateText(request).PollUntilCompleted().Result;
            Console.WriteLine($"Total Characters: {response.TotalCharacters}");
            Console.WriteLine($"Translated Characters: {response.TranslatedCharacters}");
        }
    }

    // [END translate_v3_batch_translate_text_with_glossary]
}
