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

using CommandLine;
// [START translate_v3_batch_translate_text_with_model]
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class TranslateV3BatchTranslateTextWithModel
    {
        /// <summary>
        /// Batch translate text using AutoML Translation model
        /// </summary>
        /// <param name="targetLanguage">Required. Specify up to 10 language codes here.</param>
        /// <param name="sourceLanguage">Required. Source language code.</param>
        /// <param name="modelId">The models to use for translation. Map's key is target language
        /// code.</param>
        public static void BatchTranslateTextWithModelSample(string inputUri, string outputUri, string projectId, string location, string targetLanguage, string sourceLanguage, string modelId)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            string modelPath = $"projects/{projectId}/locations/{location}/models/{modelId}";

            BatchTranslateTextRequest request = new BatchTranslateTextRequest
            {
                ParentAsLocationName = new LocationName(projectId, location),
                SourceLanguageCode = sourceLanguage,
                TargetLanguageCodes =
                {
                    // Required. Specify up to 10 language codes here.
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
                    // The models to use for translation. Map's key is target language code.
                    { targetLanguage,  modelPath},
                },
            };
            // Poll until the returned long-running operation is completed.
            BatchTranslateResponse response = translationServiceClient.BatchTranslateText(request).PollUntilCompleted().Result;
            // Display the translation for each input text provided
            Console.WriteLine($"Total Characters: {response.TotalCharacters}");
            Console.WriteLine($"Translated Characters: {response.TranslatedCharacters}");
        }
    }

    // [END translate_v3_batch_translate_text_with_model]
}
