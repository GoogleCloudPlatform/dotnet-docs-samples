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

// This is a generated sample ("LongRunningRequestPollUntilComplete", "translate_v3_batch_translate_text")

using CommandLine;
// [START translate_v3_batch_translate_text]
using System;
using System.Collections.Generic;
using Google.Cloud.Translate.V3;

namespace TranslateV3Samples
{
    public static class TranslateV3BatchTranslateText
    {
        /// <summary>
        /// Batch translate text
        /// </summary>
        public static void BatchTranslateTextSample(string inputUri, string outputUri, string projectId, string location, string sourceLanguage, string targetLanguage)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            BatchTranslateTextRequest request = new BatchTranslateTextRequest
            {
                ParentAsLocationName = new LocationName(projectId, location),
                SourceLanguageCode = sourceLanguage,
                TargetLanguageCodes =
                {
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
            };
            // Poll until the returned long-running operation is completed.
            BatchTranslateResponse response = translationServiceClient.BatchTranslateText(request).PollUntilCompleted().Result;
            Console.WriteLine($"Total Characters: {response.TotalCharacters}");
            Console.WriteLine($"Translated Characters: {response.TranslatedCharacters}");
        }
    }

    // [END translate_v3_batch_translate_text]

    public class TranslateV3BatchTranslateTextMain
    {
        public static void Main(string[] args)
        {
            new Parser(with => with.CaseInsensitiveEnumValues = true).ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                    TranslateV3BatchTranslateText.BatchTranslateTextSample(opts.InputUri, opts.OutputUri, opts.ProjectId, opts.Location, opts.SourceLanguage, opts.TargetLanguage));
        }

        public class Options
        {
            [Option("input_uri", Default = "gs://cloud-samples-data/text.txt")]
            public string InputUri { get; set; }

            [Option("output_uri", Default = "gs://YOUR_BUCKET_ID/path_to_store_results/")]
            public string OutputUri { get; set; }

            [Option("project_id", Default = "[Google Cloud Project ID]")]
            public string ProjectId { get; set; }

            [Option("location", Default = "us-central1")]
            public string Location { get; set; }

            [Option("source_language", Default = "en")]
            public string SourceLanguage { get; set; }

            [Option("target_language", Default = "ja")]
            public string TargetLanguage { get; set; }
        }
    }
}
