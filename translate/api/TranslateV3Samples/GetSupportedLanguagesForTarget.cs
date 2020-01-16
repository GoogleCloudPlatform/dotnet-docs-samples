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

// [START translate_v3_get_supported_languages_for_target]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class GetSupportedLanguagesForTarget
    {
        /// <summary>
        /// Listing supported languages with target language code.
        /// </summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="languageCode">Target Language Code.</param>
        public static void GetSupportedLanguagesForTargetSample(string languageCode = "en",
            string projectId = "[Google Cloud Project ID]")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            GetSupportedLanguagesRequest request = new GetSupportedLanguagesRequest
            {
                ParentAsLocationName = new LocationName(projectId, "global"),
                DisplayLanguageCode = languageCode,
            };
            SupportedLanguages response = translationServiceClient.GetSupportedLanguages(request);
            // List language codes of supported languages
            foreach (SupportedLanguage language in response.Languages)
            {
                Console.WriteLine($"Language Code: {language.LanguageCode}");
                Console.WriteLine($"Display Name: {language.DisplayName}");
            }
        }
    }

    // [END translate_v3_get_supported_languages_for_target]
}
