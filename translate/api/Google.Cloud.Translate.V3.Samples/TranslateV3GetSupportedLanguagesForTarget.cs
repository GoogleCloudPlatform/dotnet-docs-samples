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

// Generated code. DO NOT EDIT!

// This is a generated sample ("Request", "translate_v3_get_supported_languages_for_target")

using CommandLine;
// [START translate_v3_get_supported_languages_for_target]
using Google.Cloud.Translate.V3;
using System;

namespace Google.Cloud.Translate.V3.Samples
{
    public class TranslateV3GetSupportedLanguagesForTarget
    {
        /// <summary>
        /// Listing supported languages with target language name
        /// </summary>
        public static void SampleGetSupportedLanguages(string languageCode, string projectId)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            // TODO(developer): Uncomment and set the following variables
            // string languageCode = "en"
            // string projectId = "[Google Cloud Project ID]"
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

    public class TranslateV3GetSupportedLanguagesForTargetMain
    {
        public static void Main(string[] args)
        {
            new Parser(with => with.CaseInsensitiveEnumValues = true).ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                    TranslateV3GetSupportedLanguagesForTarget.SampleGetSupportedLanguages(opts.LanguageCode, opts.ProjectId));
        }

        public class Options
        {
            [Option("language_code", Default = "en")]
            public string LanguageCode { get; set; }

            [Option("project_id", Default = "[Google Cloud Project ID]")]
            public string ProjectId { get; set; }
        }
    }
}
