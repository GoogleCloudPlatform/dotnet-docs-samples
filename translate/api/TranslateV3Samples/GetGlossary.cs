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

// [START translate_v3_get_glossary]

using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class GetGlossary
    {
        /// <summary>
        /// Retrieves a glossary.
        /// </summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="glossaryId">Glossary ID.</param>
        public static void GetGlossarySample(string projectId = "[Google Cloud Project ID]",
            string glossaryId = "[YOUR_GLOSSARY_ID]")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
            GetGlossaryRequest request = new GetGlossaryRequest
            {
                GlossaryName = new GlossaryName(projectId, "us-central1", glossaryId),
            };
            Glossary response = translationServiceClient.GetGlossary(request);
            Console.WriteLine($"Glossary name: {response.Name}");
            Console.WriteLine($"Entry count: {response.EntryCount}");
            Console.WriteLine($"Input URI: {response.InputConfig.GcsSource.InputUri}");
        }
    }

    // [END translate_v3_get_glossary]
}
