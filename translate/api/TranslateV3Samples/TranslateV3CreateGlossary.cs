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

// This is a generated sample ("LongRunningRequestPollUntilComplete", "translate_v3_create_glossary")

using CommandLine;
// [START translate_v3_create_glossary]
using Google.Cloud.Translate.V3;
using System;
using System.Collections.Generic;

namespace TranslateV3Samples
{
    public static class TranslateV3CreateGlossary
    {
        /// <summary>
        /// Create Glossary
        /// </summary>
        public static void CreateGlossarySample(string projectId, string glossaryId, string inputUri)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            // TODO(developer): Uncomment and set the following variables
            // string projectId = "[Google Cloud Project ID]"
            // string glossaryId = "my_glossary_id_123"
            // string inputUri = "gs://cloud-samples-data/translation/glossary.csv"
            CreateGlossaryRequest request = new CreateGlossaryRequest
            {
                ParentAsLocationName = new LocationName(projectId, "us-central1"),
                Parent = new LocationName(projectId, "us-central1").ToString(),
                Glossary = new Glossary
                {
                    Name = new GlossaryName(projectId, "us-central1", glossaryId).ToString(),
                    LanguageCodesSet = new Glossary.Types.LanguageCodesSet
                    {
                        LanguageCodes =
                        {
                            "en", // source lang
                            "ja", // target lang
                        },
                    },
                    InputConfig = new GlossaryInputConfig
                    {
                        GcsSource = new GcsSource
                        {
                            InputUri = inputUri,
                        },
                    },
                },
            };
            // Poll until the returned long-running operation is complete
            Glossary response = translationServiceClient.CreateGlossary(request).PollUntilCompleted().Result;
            Console.WriteLine("Created Glossary.");
            Console.WriteLine($"Glossary name: {response.Name}");
            Console.WriteLine($"Entry count: {response.EntryCount}");
            Console.WriteLine($"Input URI: {response.InputConfig.GcsSource.InputUri}");
        }
    }

    // [END translate_v3_create_glossary]

    public class TranslateV3CreateGlossaryMain
    {
        public static void Main(string[] args)
        {
            new Parser(with => with.CaseInsensitiveEnumValues = true).ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                    TranslateV3CreateGlossary.CreateGlossarySample(opts.ProjectId, opts.GlossaryId, opts.InputUri));
        }

        public class Options
        {
            [Option("project_id", Default = "[Google Cloud Project ID]")]
            public string ProjectId { get; set; }

            [Option("glossary_id", Default = "my_glossary_id_123")]
            public string GlossaryId { get; set; }

            [Option("input_uri", Default = "gs://cloud-samples-data/translation/glossary.csv")]
            public string InputUri { get; set; }
        }
    }
}
