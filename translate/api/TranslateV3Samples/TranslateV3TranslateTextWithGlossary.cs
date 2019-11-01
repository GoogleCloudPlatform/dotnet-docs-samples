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
using Google.Cloud.Translate.V3;
using System;
using System.Collections.Generic;

namespace GoogleCloudSamples
{
    public static class TranslateV3TranslateTextWithGlossary
    {
        /// <summary>
        /// Translates a given text using a glossary.
        /// </summary>
        /// <param name="text">The content to translate in string format</param>
        /// <param name="sourceLanguage">Optional. The BCP-47 language code of the input text.</param>
        /// <param name="targetLanguage">Required. The BCP-47 language code to use for translation.</param>
        /// <param name="glossaryId">Specifies the glossary used for this translation.</param>
        public static void TranslateTextWithGlossarySample(string text, string sourceLanguage, string targetLanguage, string projectId, string glossaryId)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();

            string glossaryPath = $"projects/{projectId}/locations/{"us-central1"}/glossaries/{glossaryId}";
            TranslateTextRequest request = new TranslateTextRequest
            {
                Contents =
                {
                    // The content to translate in string format
                    text,
                },
                TargetLanguageCode = targetLanguage,
                ParentAsLocationName = new LocationName(projectId, "us-central1"),
                SourceLanguageCode = sourceLanguage,
                GlossaryConfig = new TranslateTextGlossaryConfig
                {
                    // Specifies the glossary used for this translation.
                    Glossary = glossaryPath,
                },
                MimeType = "text/plain",
            };
            TranslateTextResponse response = translationServiceClient.TranslateText(request);
            // Display the translation for each input text provided
            foreach (Translation translation in response.GlossaryTranslations)
            {
                Console.WriteLine($"Translated text: {translation.TranslatedText}");
            }
        }
    }

    // [END translate_v3_translate_text_with_glossary]
}
