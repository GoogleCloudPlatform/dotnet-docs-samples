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

// [START translate_v3_list_glossary]

using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class ListGlossary
    {
        /// <summary>
        /// Lists all the glossaries for a given project.
        /// </summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        public static void ListGlossariesSample(string projectId = "[Google Cloud Project ID]")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            ListGlossariesRequest request = new ListGlossariesRequest
            {
                ParentAsLocationName = new LocationName(projectId, "us-central1"),
            };
            PagedEnumerable<ListGlossariesResponse, Glossary> response = translationServiceClient.ListGlossaries(request);
            // Iterate over glossaries and display each glossary's details.
            foreach (Glossary item in response)
            {
                Console.WriteLine($"Glossary name: {item.Name}");
                Console.WriteLine($"Entry count: {item.EntryCount}");
                Console.WriteLine($"Input URI: {item.InputConfig.GcsSource.InputUri}");
            }
        }
    }

    // [END translate_v3_list_glossary]
}
