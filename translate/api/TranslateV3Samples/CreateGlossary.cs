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

// [START translate_v3_create_glossary]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Translate.V3;
using System;

namespace GoogleCloudSamples
{
    public static class CreateGlossary
    {
        /// <summary>
        /// Create Glossary.
        /// </summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="glossaryId">Your glossary display name.</param>
        /// <param name="inputUri">Google Cloud Storage URI where glossary is stored in csv format.</param>
        public static void CreateGlossarySample(string projectId = "[Google Cloud Project ID]",
            string glossaryId = "your-glossary-display-name",
            string inputUri = "gs://cloud-samples-data/translation/glossary_ja.csv")
        {
            TranslationServiceClient translationServiceClient = TranslationServiceClient.Create();
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
}
