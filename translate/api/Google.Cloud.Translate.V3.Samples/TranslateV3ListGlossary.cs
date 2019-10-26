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

// This is a generated sample ("RequestPagedAll", "translate_v3_list_glossary")

using CommandLine;
// [START translate_v3_list_glossary]
using Google.Api.Gax;
using Google.Cloud.Translate.V3;
using System;

namespace Google.Cloud.Translate.V3.Samples
{
    public class TranslateV3ListGlossary
    {
        /// <summary>
        /// List Glossaries
        /// </summary>
        public static void SampleListGlossaries(string projectId)
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            // string project = "[Google Cloud Project ID]"
            ListGlossariesRequest request = new ListGlossariesRequest
            {
                ParentAsLocationName = new LocationName(projectId, "us-central1"),
            };
            PagedEnumerable<ListGlossariesResponse, Glossary> response = translationServiceClient.ListGlossaries(request);
            // Iterate over pages (of server-defined size), performing one RPC per page
            foreach (Glossary item in response)
            {
                Console.WriteLine($"Glossary name: {item.Name}");
                Console.WriteLine($"Entry count: {item.EntryCount}");
                Console.WriteLine($"Input URI: {item.InputConfig.GcsSource.InputUri}");
            }
        }
    }

    // [END translate_v3_list_glossary]

    public class TranslateV3ListGlossaryMain
    {
        public static void Main(string[] args)
        {
            new Parser(with => with.CaseInsensitiveEnumValues = true).ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                    TranslateV3ListGlossary.SampleListGlossaries(opts.ProjectId));
        }

        public class Options
        {
            [Option("project_id", Default = "[Google Cloud Project ID]")]
            public string ProjectId { get; set; }
        }
    }
}
