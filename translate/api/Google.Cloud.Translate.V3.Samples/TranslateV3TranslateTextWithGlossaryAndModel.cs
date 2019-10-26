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

// This is a generated sample ("Request", "translate_v3_translate_text_with_glossary_and_model")

using CommandLine;
// [START translate_v3_translate_text_with_glossary_and_model]
using Google.Cloud.Translate.V3;
using System;
using System.Collections.Generic;

namespace Google.Cloud.Translate.V3.Samples
{
    public class TranslateV3TranslateTextWithGlossaryAndModel
    {
        /// <summary>
        /// Translating Text with Glossary and Model
        /// </summary>
        /// <param name="modelId">The `model` type requested for this translation.</param>
        /// <param name="glossaryId">Specifies the glossary used for this translation.</param>
        /// <param name="text">The content to translate in string format</param>
        /// <param name="targetLanguage">Required. The BCP-47 language code to use for translation.</param>
        /// <param name="sourceLanguage">Optional. The BCP-47 language code of the input text.</param>
        public static void SampleTranslateTextWithGlossaryAndModel(string modelId, string glossaryId, string text,
            string targetLanguage, string sourceLanguage, string projectId, string location)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            // string modelPath = "projects/[PROJECT ID]/locations/[LOCATION ID]/models/[MODEL ID]"
            // string glossaryPath = "projects/[YOUR_PROJECT_ID]/locations/[LOCATION]/glossaries/[YOUR_GLOSSARY_ID]"
            // string text = "Hello, world!"
            // string targetLanguage = "fr"
            // string sourceLanguage = "en"
            // string projectId = "[Google Cloud Project ID]"
            // string location = "global"
            string modelPath = $"projects/{projectId}/locations/{location}/models/{modelId}";
            string glossaryPath = $"projects/{projectId}/locations/{location}/glossaries/{glossaryId}";

            TranslateTextRequest request = new TranslateTextRequest
            {
                Contents =
                {
                    // The content to translate in string format
                    text,
                },
                TargetLanguageCode = targetLanguage,
                ParentAsLocationName = new LocationName(projectId, location),
                Model = modelPath,
                GlossaryConfig = new TranslateTextGlossaryConfig
                {
                    // Specifies the glossary used for this translation.
                    Glossary = glossaryPath,
                },
                SourceLanguageCode = sourceLanguage,
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

    // [END translate_v3_translate_text_with_glossary_and_model]

    public class TranslateV3TranslateTextWithGlossaryAndModelMain
    {
        public static void Main(string[] args)
        {
            new Parser(with => with.CaseInsensitiveEnumValues = true).ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                    TranslateV3TranslateTextWithGlossaryAndModel.SampleTranslateTextWithGlossaryAndModel(opts.ModelId, opts.GlossaryId,
                    opts.Text, opts.TargetLanguage, opts.SourceLanguage, opts.ProjectId, opts.Location));
        }

        public class Options
        {
            [Option("model_id", Default = "[MODEL ID]")]
            public string ModelId { get; set; }

            [Option("glossary_id", Default = "[YOUR_GLOSSARY_ID]")]
            public string GlossaryId { get; set; }

            [Option("text", Default = "Hello, world!")]
            public string Text { get; set; }

            [Option("target_language", Default = "fr")]
            public string TargetLanguage { get; set; }

            [Option("source_language", Default = "en")]
            public string SourceLanguage { get; set; }

            [Option("project_id", Default = "[Google Cloud Project ID]")]
            public string ProjectId { get; set; }

            [Option("location", Default = "global")]
            public string Location { get; set; }
        }
    }
}
