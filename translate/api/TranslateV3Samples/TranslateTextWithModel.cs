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

// [START translate_v3_translate_text_with_model]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class TranslateTextWithModel
    {
        /// <summary>
        /// Translates a given text to a target language with custom model.
        /// </summary>
        /// <param name="modelId">Translation Model ID.</param>
        /// <param name="text">The content to translate.t</param>
        /// <param name="targetLanguage">Required. Target language code.</param>
        /// <param name="sourceLanguage">Optional. Source language code.</param>
        /// <param name="projectId"> Google Project ID.</param>
        /// <param name="location"> Region.</param>
        public static void TranslateTextWithModelSample(
            string modelId = "[YOUR_MODEL_ID]",
            string text = "[TEXT_TO_TRANSLATE]",
            string targetLanguage = "ja",
            string sourceLanguage = "en",
            string projectId = "[Google Cloud Project ID]",
            string location = "us-central1")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            string modelPath = $"projects/{projectId}/locations/{location}/models/{modelId}";

            TranslateTextRequest request = new TranslateTextRequest
            {
                Contents =
                {
                    // The content to translate.
                    text,
                },
                TargetLanguageCode = targetLanguage,
                ParentAsLocationName = new LocationName(projectId, location),
                Model = modelPath,
                SourceLanguageCode = sourceLanguage,
                MimeType = "text/plain",
            };
            TranslateTextResponse response = translationServiceClient.TranslateText(request);
            // Display the translation for each input text provided
            foreach (Translation translation in response.Translations)
            {
                Console.WriteLine($"Translated text: {translation.TranslatedText}");
            }
        }
    }

    // [END translate_v3_translate_text_with_model]
}
