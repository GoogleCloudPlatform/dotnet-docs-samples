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

// This is a generated sample ("LongRunningRequestPollUntilComplete", "translate_v3_delete_glossary")

using CommandLine;
// [START translate_v3_delete_glossary]
using Google.Cloud.Translate.V3;
using System;

namespace TranslateV3Samples
{
    public static class TranslateV3DeleteGlossary
    {
        /// <summary>
        /// Delete Glossary
        /// </summary>
        public static void DeleteGlossarySample(string projectId, string glossaryId)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            DeleteGlossaryRequest request = new DeleteGlossaryRequest
            {
                GlossaryName = new GlossaryName(projectId, "us-central1", glossaryId),
            };
            // Poll until the returned long-running operation is complete
            DeleteGlossaryResponse response = translationServiceClient.DeleteGlossary(request).PollUntilCompleted().Result;
            Console.WriteLine("Deleted Glossary.");
        }
    }

    // [END translate_v3_delete_glossary]

    public class TranslateV3DeleteGlossaryMain
    {
        public static void Main(string[] args)
        {
            new Parser(with => with.CaseInsensitiveEnumValues = true).ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                    TranslateV3DeleteGlossary.DeleteGlossarySample(opts.ProjectId, opts.GlossaryId));
        }

        public class Options
        {
            [Option("project_id", Default = "[Google Cloud Project ID]")]
            public string ProjectId { get; set; }

            [Option("glossary_id", Default = "[Glossary ID]")]
            public string GlossaryId { get; set; }
        }
    }
}
