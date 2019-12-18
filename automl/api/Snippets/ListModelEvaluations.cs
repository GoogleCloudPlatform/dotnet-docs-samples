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
    [Verb("list_model_evaluations", HelpText = "List model evaluations")]
    public class ListModelEvaluationsOptions : ListModelOptions
    {
        [Value(1, HelpText = "Your project ID")]
        public string ModelId { get; set; }
    }

    class AutoMLListModelEvaluations
    {
        // [START automl_language_entity_extraction_list_model_evaluations]
        // [START automl_language_sentiment_analysis_list_model_evaluations]
        // [START automl_language_text_classification_list_model_evaluations]
        // [START automl_translate_list_model_evaluations]
        // [START automl_vision_classification_list_model_evaluations]
        // [START automl_vision_object_detection_list_model_evaluations]
        /// <summary>
        /// Demonstrates using the AutoML client to list model evaluations.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="modelId">the Id of the model.</param>
        public static object ListModelEvaluations(string projectId = "YOUR-PROJECT-ID",
            string modelId = "YOUR-MODEL-ID")
        {
            // Initialize the client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests.
            AutoMlClient client = AutoMlClient.Create();

            // Get the full path of the model.
            string modelFullId = ModelName.Format(projectId, "us-central1", modelId);

            // Create list models request.
            ListModelEvaluationsRequest listModlesRequest = new ListModelEvaluationsRequest
            {
                Parent = modelFullId
            };

            // List all the model evaluations in the model by applying filter.
            Console.WriteLine("List of model evaluations:");
            foreach (ModelEvaluation modelEvaluation in client.ListModelEvaluations(listModlesRequest))
            {
                Console.WriteLine($"Model Evaluation Name: {modelEvaluation.Name}");
                Console.WriteLine($"Model Annotation Spec Id: {modelEvaluation.AnnotationSpecId}");
                Console.WriteLine("Create Time:");
                Console.WriteLine($"\tseconds: {modelEvaluation.CreateTime.Seconds}");
                Console.WriteLine($"\tnanos: {modelEvaluation.CreateTime.Nanos / 1e9}");
                Console.WriteLine(
                    $"Evalution Example Count: {modelEvaluation.EvaluatedExampleCount}");
                // [END automl_language_sentiment_analysis_list_model_evaluations]
                // [END automl_language_text_classification_list_model_evaluations]
                // [END automl_translate_list_model_evaluations]
                // [END automl_vision_classification_list_model_evaluations]
                // [END automl_vision_object_detection_list_model_evaluations]
                Console.WriteLine(
                    $"Entity Extraction Model Evaluation Metrics: {modelEvaluation.TextExtractionEvaluationMetrics}");
                // [END automl_language_entity_extraction_list_model_evaluations]

                // [START automl_language_sentiment_analysis_list_model_evaluations]
                Console.WriteLine(
                    $"Sentiment Analysis Model Evaluation Metrics: {modelEvaluation.TextSentimentEvaluationMetrics}");
                // [END automl_language_sentiment_analysis_list_model_evaluations]

                // [START automl_language_text_classification_list_model_evaluations]
                // [START automl_vision_classification_list_model_evaluations]
                Console.WriteLine(
                    $"Classificatio Model Evaluation Metrics: {modelEvaluation.ClassificationEvaluationMetrics}");
                // [END automl_language_text_classification_list_model_evaluations]
                // [END automl_vision_classification_list_model_evaluations]

                // [START automl_translate_list_model_evaluations]
                Console.WriteLine(
                    $"Model Evaluation Metrics: {modelEvaluation.TranslationEvaluationMetrics}");
                // [END automl_translate_list_model_evaluations]

                // [START automl_vision_object_detection_list_model_evaluations]
                Console.WriteLine(
                    $"Object Detection Model Evaluation Metrics: {modelEvaluation.ImageObjectDetectionEvaluationMetrics}");
                // [START automl_language_entity_extraction_list_model_evaluations]
                // [START automl_language_sentiment_analysis_list_model_evaluations]
                // [START automl_language_text_classification_list_model_evaluations]
                // [START automl_translate_list_model_evaluations]
                // [START automl_vision_classification_list_model_evaluations]
            }
            return 0;
        }
        // [END automl_language_entity_extraction_list_model_evaluations]
        // [END automl_language_sentiment_analysis_list_model_evaluations]
        // [END automl_language_text_classification_list_model_evaluations]
        // [END automl_translate_list_model_evaluations]
        // [END automl_vision_classification_list_model_evaluations]
        // [END automl_vision_object_detection_list_model_evaluations]
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((ListModelEvaluationsOptions opts) =>
                     AutoMLListModelEvaluations.ListModelEvaluations(opts.ProjectID, opts.ModelId));
        }
    }
}