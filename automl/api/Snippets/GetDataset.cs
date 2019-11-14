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

// [START automl_language_entity_extraction_get_dataset]
// [START automl_language_sentiment_analysis_get_dataset]
// [START automl_language_text_classification_get_dataset]
// [START automl_translate_get_dataset]
// [START automl_vision_classification_get_dataset]
// [START automl_vision_object_detection_get_dataset]

using Google.Cloud.AutoML.V1;
using System;

class AutoMLGetDataset
{
    // TODO: complete summary
    public static void GetDataset(string projectId = "YOUR-PROJECT-ID",
        string datasetId = "YOUR-DATASET-ID")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests. After completing all of your requests, call
        // the "close" method on the client to safely clean up any remaining background resources.
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
        Console.WriteLine(
            $"\tSource language code: {dataset.TranslationDatasetMetadata.SourceLanguageCode}");
        Console.WriteLine(
            $"\tTarget language code: {dataset.TranslationDatasetMetadata.TargetLanguageCode}");
        // [END automl_translate_get_dataset]

        // [START automl_vision_classification_get_dataset]
        Console.WriteLine(
            $"Image classification dataset metadata: {dataset.ImageClassificationDatasetMetadata}");
        // [END automl_vision_classification_get_dataset]

        // [START automl_vision_object_detection_get_dataset]
        Console.WriteLine(
            $"Image object detection dataset metadata: {dataset.ImageObjectDetectionDatasetMetadata}");
        // [START automl_language_entity_extraction_get_dataset]
        // [START automl_language_sentiment_analysis_get_dataset]
        // [START automl_language_text_classification_get_dataset]
        // [START automl_translate_get_dataset]
        // [START automl_vision_classification_get_dataset]
    }

}

// [END automl_language_entity_extraction_get_dataset]
// [END automl_language_sentiment_analysis_get_dataset]
// [END automl_language_text_classification_get_dataset]
// [END automl_translate_get_dataset]
// [END automl_vision_classification_get_dataset]
// [END automl_vision_object_detection_get_dataset]