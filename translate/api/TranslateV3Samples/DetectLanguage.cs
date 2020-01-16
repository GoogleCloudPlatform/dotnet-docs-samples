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

// [START translate_v3_detect_language]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class DetectLanguage
    {
        /// <summary>
        /// Detects the language of a given text.
        /// </summary>
        /// <param name="text">The text string for performing language detection</param>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        public static void DetectLanguageSample(string text = "[TEXT_STRING_FOR_DETECTION]",
            string projectId = "[Google Cloud Project ID]")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            DetectLanguageRequest request = new DetectLanguageRequest
            {
                ParentAsLocationName = new LocationName(projectId, "global"),
                Content = text,
                MimeType = "text/plain",
            };
            DetectLanguageResponse response = translationServiceClient.DetectLanguage(request);
            // Display list of detected languages sorted by detection confidence.
            // The most probable language is first.
            foreach (DetectedLanguage language in response.Languages)
            {
                // The language detected
                Console.WriteLine($"Language code: {language.LanguageCode}");
                // Confidence of detection result for this language
                Console.WriteLine($"Confidence: {language.Confidence}");
            }
        }
    }

    // [END translate_v3_detect_language]
}
