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
    public class AutoMLListDatasets
    {
        [Verb("list_datasets", HelpText = "List all datasets")]
        public class ListDatasetsOptions : BaseOptions
        {
        }


        // [START automl_language_entity_extraction_list_datasets]
        // [START automl_language_sentiment_analysis_list_datasets]
        // [START automl_language_text_classification_list_datasets]
        // [START automl_translate_list_datasets]
        // [START automl_vision_classification_list_datasets]
        // [START automl_vision_object_detection_list_datasets]

        /// <summary>
        /// Demonstrates using the AutoML client to list all datasets.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        public static object ListDatasets(string projectId = "YOUR-PROJECT-ID")
        {
            // Initialize the client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests.
            AutoMlClient client = AutoMlClient.Create();

            // A resource that represents Google Cloud Platform location.
            string projectLocation = LocationName.Format(projectId, "us-central1");
            ListDatasetsRequest request = new ListDatasetsRequest
            {
                Parent = projectLocation
            };

            // List all the datasets available in the region by applying filter.
            Console.WriteLine("List of datasets:");
            foreach (Dataset dataset in client.ListDatasets(request))
            {
                // Display the dataset information
                Console.WriteLine($"Dataset name: {dataset.Name}");
                // To get the dataset id, you have to parse it out of the `name` field. As dataset Ids are
                // required for other methods.
                // Name Form: `projects/{project_id}/locations/{location_id}/datasets/{dataset_id}`
                string[] names = dataset.Name.Split("/");
                string retrievedDatasetId = names[names.Length - 1];
                Console.WriteLine($"Dataset id: {retrievedDatasetId}");
                Console.WriteLine($"Dataset display name: {dataset.DisplayName}");
                Console.WriteLine("Dataset create time:");
                Console.WriteLine($"\tseconds: {dataset.CreateTime.Seconds}");
                Console.WriteLine($"\tnanos: {dataset.CreateTime.Nanos}");
                // [END automl_language_sentiment_analysis_list_datasets]
                // [END automl_language_text_classification_list_datasets]
                // [END automl_translate_list_datasets]
                // [END automl_vision_classification_list_datasets]
                // [END automl_vision_object_detection_list_datasets]
                Console.WriteLine(
                    $"Text extraction dataset metadata: {dataset.TextExtractionDatasetMetadata}");
                // [END automl_language_entity_extraction_list_datasets]

                // [START automl_language_sentiment_analysis_list_datasets]
                Console.WriteLine(
                    $"Text sentiment dataset metadata: {dataset.TextSentimentDatasetMetadata}");
                // [END automl_language_sentiment_analysis_list_datasets]

                // [START automl_language_text_classification_list_datasets]
                Console.WriteLine(
                    $"Text classification dataset metadata: {dataset.TextClassificationDatasetMetadata}");
                // [END automl_language_text_classification_list_datasets]

                // [START automl_translate_list_datasets]
                Console.WriteLine("Translation dataset metadata:");

                //TODO: throwing null pointer reference!.

                //Console.WriteLine(
                //    $"\tSource language code: {dataset.TranslationDatasetMetadata.SourceLanguageCode}");
                //Console.WriteLine(
                //    $"\tTarget language code: {dataset.TranslationDatasetMetadata.TargetLanguageCode}");
                // [END automl_translate_list_datasets]

                // [START automl_vision_classification_list_datasets]
                Console.WriteLine(
                    $"Image classification dataset metadata: {dataset.ImageClassificationDatasetMetadata}");
                // [END automl_vision_classification_list_datasets]

                // [START automl_vision_object_detection_list_datasets]
                Console.WriteLine(
                    $"Image object detection dataset metadata: {dataset.ImageObjectDetectionDatasetMetadata}");

                // [START automl_language_entity_extraction_list_datasets]
                // [START automl_language_sentiment_analysis_list_datasets]
                // [START automl_language_text_classification_list_datasets]
                // [START automl_translate_list_datasets]
                // [START automl_vision_classification_list_datasets]
            }
            return 0;
        }
        // [END automl_language_entity_extraction_list_datasets]
        // [END automl_language_sentiment_analysis_list_datasets]
        // [END automl_language_text_classification_list_datasets]
        // [END automl_translate_list_datasets]
        // [END automl_vision_classification_list_datasets]
        // [END automl_vision_object_detection_list_datasets]
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((ListDatasetsOptions opts) =>
                     AutoMLListDatasets.ListDatasets(opts.ProjectID));
        }
    }
}
