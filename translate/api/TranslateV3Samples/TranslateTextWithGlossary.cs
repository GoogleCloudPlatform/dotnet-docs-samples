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

// [START translate_v3_translate_text_with_glossary]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class TranslateTextWithGlossary
    {
        /// <summary>
        /// Translates a given text to a target language using glossary.
        /// </summary>
        /// <param name="text">The content to translate.</param>
        /// <param name="sourceLanguage">Optional. Source language code.</param>
        /// <param name="targetLanguage">Required. Target language code.</param>
        /// <param name="glossaryId">Translation Glossary ID.</param>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        public static void TranslateTextWithGlossarySample(
            string text = "[TEXT_TO_TRANSLATE]",
            string sourceLanguage = "en",
            string targetLanguage = "ja",
            string projectId = "[Google Cloud Project ID]",
            string glossaryId = "[YOUR_GLOSSARY_ID]")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();

            string glossaryPath = $"projects/{projectId}/locations/{"us-central1"}/glossaries/{glossaryId}";
            TranslateTextRequest request = new TranslateTextRequest
            {
                Contents =
                {
                    // The content to translate.
                    text,
                },
                TargetLanguageCode = targetLanguage,
                ParentAsLocationName = new LocationName(projectId, "us-central1"),
                SourceLanguageCode = sourceLanguage,
                GlossaryConfig = new TranslateTextGlossaryConfig
                {
                    // Translation Glossary Path.
                    Glossary = glossaryPath,
                },
                MimeType = "text/plain",
            };
            TranslateTextResponse response = translationServiceClient.TranslateText(request);
            // Display the translation for given content.
            foreach (Translation translation in response.GlossaryTranslations)
            {
                Console.WriteLine($"Translated text: {translation.TranslatedText}");
            }
        }
    }

    // [END translate_v3_translate_text_with_glossary]
}
