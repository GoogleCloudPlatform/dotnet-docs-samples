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
    [Verb("get_dataset", HelpText = "Retrieve a dataset for AutoML Translation")]
    public class GetDatasetOptions : BaseOptions
    {
        [Value(1, HelpText = "ID of dataset to retreive.")]
        public string DatasetID { get; set; }
    }

    class AutoMLGetDataset
    {
        // [START automl_language_entity_extraction_get_dataset]
        // [START automl_language_sentiment_analysis_get_dataset]
        // [START automl_language_text_classification_get_dataset]
        // [START automl_translate_get_dataset]
        // [START automl_vision_classification_get_dataset]
        // [START automl_vision_object_detection_get_dataset]
        /// <summary>
        /// Demonstrates using the AutoML client to get a dataset by ID.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="datasetId">the Id of the dataset.</param>
        public static object GetDataset(string projectId = "YOUR-PROJECT-ID",
            string datasetId = "YOUR-DATASET-ID")
        {
            // Initialize the client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests.
            AutoMlClient client = AutoMlClient.Create();

            // Get the full path of the model.
            string datasetFullId = DatasetName.Format(projectId, "us-central1", datasetId);
            Dataset dataset = client.GetDataset(datasetFullId);

            // Display the dataset information
            Console.WriteLine($"Dataset name: {dataset.Name}");
            // To get the dataset id, you have to parse it out of the `name` field. As dataset Ids are
            // required for other methods.
            // Name Form: `projects/{project_id}/locations/{location_id}/datasets/{dataset_id}`
            string[] names = dataset.Name.Split("/");
            string retrievedDatasetId = names[names.Length - 1];
            Console.WriteLine($"Dataset id: {retrievedDatasetId}");
            Console.WriteLine($"Dataset display name: { dataset.DisplayName}");
            Console.WriteLine("Dataset create time:");
            Console.WriteLine($"\tseconds: {dataset.CreateTime.Seconds}");
            Console.WriteLine($"\tnanos: {dataset.CreateTime.Nanos}");
            // [END automl_language_sentiment_analysis_get_dataset]
            // [END automl_language_text_classification_get_dataset]
            // [END automl_translate_get_dataset]
            // [END automl_vision_classification_get_dataset]
            // [END automl_vision_object_detection_get_dataset]
            Console.WriteLine(
                $"Text extraction dataset metadata: {dataset.TextExtractionDatasetMetadata}");
            // [END automl_language_entity_extraction_get_dataset]

            // [START automl_language_sentiment_analysis_get_dataset]
            Console.WriteLine(
                $"Text sentiment dataset metadata: {dataset.TextSentimentDatasetMetadata}");
            // [END automl_language_sentiment_analysis_get_dataset]

            // [START automl_language_text_classification_get_dataset]
            Console.WriteLine(
                $"Text classification dataset metadata: {dataset.TextClassificationDatasetMetadata}");
            // [END automl_language_text_classification_get_dataset]

            // [START automl_translate_get_dataset]
            Console.WriteLine($"Translation dataset metadata:");
            //TO-DO I do not know why i am Getting NullReference. It was working fine before. Commented out for now.

            //Console.WriteLine(
            //    $"\tSource language code: {dataset.TranslationDatasetMetadata.SourceLanguageCode}");
            //Console.WriteLine(
            //    $"\tTarget language code: {dataset.TranslationDatasetMetadata.TargetLanguageCode}");

            // [END automl_translate_get_dataset]

            // [START automl_vision_classification_get_dataset]
            Console.WriteLine(
                $"Image classification dataset metadata: {dataset.ImageClassificationDatasetMetadata}");
            // [END automl_vision_classification_get_dataset]

            // [START automl_vision_object_detection_get_dataset]
            Console.WriteLine(
                $"Image object detection dataset metadata: {dataset.ImageObjectDetectionDatasetMetadata}");

            return 0;
            // [START automl_language_entity_extraction_get_dataset]
            // [START automl_language_sentiment_analysis_get_dataset]
            // [START automl_language_text_classification_get_dataset]
            // [START automl_translate_get_dataset]
            // [START automl_vision_classification_get_dataset]

        }

        // [END automl_language_entity_extraction_get_dataset]
        // [END automl_language_sentiment_analysis_get_dataset]
        // [END automl_language_text_classification_get_dataset]
        // [END automl_translate_get_dataset]
        // [END automl_vision_classification_get_dataset]
        // [END automl_vision_object_detection_get_dataset]
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((GetDatasetOptions opts) =>
                     AutoMLGetDataset.GetDataset(opts.ProjectID, opts.DatasetID));
        }
    }
}
