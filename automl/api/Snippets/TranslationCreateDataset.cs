// Copyright (c) 2019 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START automl_translation_create_dataset]

using CommandLine;
using Google.Cloud.AutoML.V1;
using System;

namespace GoogleCloudSamples
{
    [Verb("create_translation_dataset", HelpText = "Create a dataset for AutoML Translation")]
    public class AutoMLTranslationCreateDatasetOptions : CreateDatasetOptions
    {
        [Value(2, HelpText = "The language to translate from")]
        public string SourceLanguage { get; set; }

        [Value(3, HelpText = "The language to translate to")]
        public string TargetLanguage { get; set; }
    }

    public class AutoMLTranslationCreateDataset
    {
        /// <summary>
        /// Demonstrates using the AutoML client to create a dataset with given display name.
        /// </summary>
        /// <param name="projectID">GCP Project ID.</param>
        /// <param name="displayName">the readable name of the dataset.</param>
        /// <param name="targetLanguage">the language to translate to</param>
        /// <param name="sourceLanguage">the language to translate from</param>
        /// <returns>Success or failure</returns>
        public static object TranslationCreateDataset(string projectID = "YOUR-PROJECT-ID",
                                                    string displayName = "YOUR-DATASET-NAME",
                                                    string targetLanguage = "TARGET-LANG-CODE",
                                                    string sourceLanguage = "SOURCE-LANG-CODE")
        {

            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            var client = AutoMlClient.Create();

            // A resource that represents Google Cloud Platform location.
            var locationName = LocationName.Format(projectID, "us-central1");

            var datasetRequest = new Dataset
            {
                DisplayName = displayName,
                TranslationDatasetMetadata = new TranslationDatasetMetadata
                {
                    SourceLanguageCode = sourceLanguage,
                    TargetLanguageCode = targetLanguage
                }
            };

            var dataset = client.CreateDatasetAsync(locationName, datasetRequest)
                                .Result.PollUntilCompleted().Result;

            // Display the dataset information.
            Console.WriteLine($"Dataset name: {dataset.Name}");
            // To get the dataset id, you have to parse it out of the `name` field. As dataset Ids are
            // required for other methods.
            // Name Form: `projects/{project_id}/locations/{location_id}/datasets/{dataset_id}`
            var names = dataset.Name.Split("/");
            var datasetId = names[names.Length - 1];
            Console.WriteLine($"Dataset id: {datasetId}");
            return 0;
        }

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((AutoMLTranslationCreateDatasetOptions opts) =>
                     AutoMLTranslationCreateDataset.TranslationCreateDataset(opts.ProjectID,
                                                                             opts.DisplayName,
                                                                             opts.TargetLanguage,
                                                                             opts.SourceLanguage));
        }
    }
}
// [END automl_translation_create_dataset]
