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

using CommandLine;
using Google.Cloud.AutoML.V1;
using System;

namespace GoogleCloudSamples
{
    [Verb("create_dataset_language_sentiment_analysis", HelpText = "Create a dataset for language sentiment analysis")]
    public class LanguageSentimentAnalysisCreateDatasetOptions : CreateDatasetOptions
    {
    }
    class AutoMLLanguageSentimentAnalysisCreateDataset
    {
        // [START automl_language_sentiment_analysis_create_dataset]
        /// <summary>
        ///  Demonstrates using the AutoML client to create a dataset.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="displayName">The name of the dataset to be created.</param>
        public static object LanguageSentimentAnalysisCreateDataset(string projectId = "YOUR-PROJECT-ID",
            string displayName = "YOUR-DATASET-NAME")
        {
            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            AutoMlClient client = AutoMlClient.Create();

            // A resource that represents Google Cloud Platform location.
            string projectLocation = LocationName.Format(projectId, "us-central1");

            TextSentimentDatasetMetadata metadata = new TextSentimentDatasetMetadata
            {
                SentimentMax = 4 // Possible max sentiment score: 1-10.
            };

            Dataset dataset = new
                Dataset
            {
                DisplayName = displayName,
                TextSentimentDatasetMetadata = metadata
            };

            Dataset createdDataset = client
                .CreateDatasetAsync(projectLocation, dataset).Result.PollUntilCompleted().Result;

            // Display the dataset information.
            Console.WriteLine($"Dataset name: {createdDataset.Name}");
            // To get the dataset id, you have to parse it out of the `name` field. As dataset Ids are
            // required for other methods.
            // Name Form: `projects/{project_id}/locations/{location_id}/datasets/{dataset_id}`
            string[] names = createdDataset.Name.Split("/");
            string datasetId = names[names.Length - 1];
            Console.WriteLine($"Dataset id: {datasetId}");

            return 0;
        }
        // [END automl_language_sentiment_analysis_create_dataset]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((LanguageSentimentAnalysisCreateDatasetOptions opts) =>
                     AutoMLLanguageSentimentAnalysisCreateDataset.LanguageSentimentAnalysisCreateDataset(opts.ProjectID,
                                                                 opts.DisplayName));
        }
    }
}