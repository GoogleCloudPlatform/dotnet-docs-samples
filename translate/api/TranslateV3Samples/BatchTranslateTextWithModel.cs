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

// [START translate_v3_batch_translate_text_with_model]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class BatchTranslateTextWithModel
    {
        /// <summary>
        /// Batch translate text using Translation model.
        /// Model can be AutoML or General[built-in] model.
        /// </summary>
        /// <param name="targetLanguage">Target language code.</param>
        /// <param name="sourceLanguage">Required. Source language code.</param>
        /// <param name="modelId">A model is used for Translation.
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="inputUri">The GCS path where input configuration is stored.</param>
        /// <param name="outputUri">The GCS path for translation output.</param>
        /// <param name="location"> Region.</param>
        public static void BatchTranslateTextWithModelSample(
            string inputUri = "gs://cloud-samples-data/translation/custom_model_text.txt'",
            string outputUri = "gs://YOUR_BUCKET_ID/path_to_store_results/",
            string projectId = "[Google Cloud Project ID]",
            string location = "us-central1",
            string targetLanguage = "ja",
            string sourceLanguage = "en",
            string modelId = "[YOUR-MODEL-ID]")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            string modelPath = $"projects/{projectId}/locations/{location}/models/{modelId}";

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
                Models =
                {
                    // A model is used for Translation.
                    { targetLanguage,  modelPath},
                },
            };
            // Poll until the returned long-running operation is completed.
            BatchTranslateResponse response = translationServiceClient.BatchTranslateText(request).PollUntilCompleted().Result;
            Console.WriteLine($"Total Characters: {response.TotalCharacters}");
            Console.WriteLine($"Translated Characters: {response.TranslatedCharacters}");
        }
    }

    // [END translate_v3_batch_translate_text_with_model]
}
