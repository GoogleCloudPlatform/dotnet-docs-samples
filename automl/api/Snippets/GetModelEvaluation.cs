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
    class AutoMLGetModelEvaluation
    {
        [Verb("get_model_evaluation", HelpText = "Get a model evaluation")]
        public class GetModelEvaluationOptions : GetModelOptions
        {
            [Value(2, HelpText = "ID of model evaluation.")]
            public string ModelEvalId { get; set; }
        }

        // [START automl_language_entity_extraction_get_model_evaluation]
        // [START automl_language_sentiment_analysis_get_model_evaluation]
        // [START automl_language_text_classification_get_model_evaluation]
        // [START automl_translate_get_model_evaluation]
        // [START automl_vision_classification_get_model_evaluation]
        // [START automl_vision_object_detection_get_model_evaluation]
        /// <summary>
        /// Demonstrates using the AutoML client to get model evaluations.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="modelId">the Id of the model.</param>
        /// <param name="modelEvaluationId">the Id of your model evaluation.</param>
        public static object GetModelEvaluation(string projectId = "YOUR-PROJECT-ID",
            string modelId = "YOUR-MODEL-ID",
            string modelEvaluationId = " YOUR-MODEL-EVAL-ID")
        {
            // Initialize the client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests.
            AutoMlClient client = AutoMlClient.Create();

            // Get the full path of the model evaluation.
            string modelEvaluationFullId =
                ModelEvaluationName.Format(projectId, "us-central1", modelId, modelEvaluationId);

            // Get complete detail of the model evaluation.
            ModelEvaluation modelEvaluation = client.GetModelEvaluation(modelEvaluationFullId);

            Console.WriteLine($"Model Evaluation Name: {modelEvaluation.Name}");
            Console.WriteLine($"Model Annotation Spec Id: {modelEvaluation.AnnotationSpecId}");
            Console.WriteLine("Create Time:");
            Console.WriteLine($"\tseconds: {modelEvaluation.CreateTime.Seconds}");
            Console.WriteLine($"\tnanos: {modelEvaluation.CreateTime.Nanos / 1e9}");
            Console.WriteLine(
                $"Evalution Example Count: {modelEvaluation.EvaluatedExampleCount}");

            // [END automl_language_sentiment_analysis_get_model_evaluation]
            // [END automl_language_text_classification_get_model_evaluation]
            // [END automl_translate_get_model_evaluation]
            // [END automl_vision_classification_get_model_evaluation]
            // [END automl_vision_object_detection_get_model_evaluation]
            Console.WriteLine(
                $"Entity Extraction Model Evaluation Metrics: {modelEvaluation.TextExtractionEvaluationMetrics}");
            // [END automl_language_entity_extraction_get_model_evaluation]

            // [START automl_language_sentiment_analysis_get_model_evaluation]
            Console.WriteLine(
                $"Sentiment Analysis Model Evaluation Metrics: {modelEvaluation.TextSentimentEvaluationMetrics}");
            // [END automl_language_sentiment_analysis_get_model_evaluation]

            // [START automl_language_text_classification_get_model_evaluation]
            // [START automl_vision_classification_get_model_evaluation]
            Console.WriteLine(
                $"Classificatio Model Evaluation Metrics: {modelEvaluation.ClassificationEvaluationMetrics}");
            // [END automl_language_text_classification_get_model_evaluation]
            // [END automl_vision_classification_get_model_evaluation]
            // [START automl_translate_get_model_evaluation]
            Console.WriteLine(
                $"Model Evaluation Metrics: {modelEvaluation.TranslationEvaluationMetrics}");
            // [END automl_translate_get_model_evaluation]

            // [START automl_vision_object_detection_get_model_evaluation]
            Console.WriteLine(
                $"Object Detection Model Evaluation Metrics: {modelEvaluation.ImageObjectDetectionEvaluationMetrics}");
            // [START automl_language_entity_extraction_get_model_evaluation]
            // [START automl_language_sentiment_analysis_get_model_evaluation]
            // [START automl_language_text_classification_get_model_evaluation]
            // [START automl_translate_get_model_evaluation]
            // [START automl_vision_classification_get_model_evaluation]
            return 0;
        }

        // [END automl_language_entity_extraction_get_model_evaluation]
        // [END automl_language_sentiment_analysis_get_model_evaluation]
        // [END automl_language_text_classification_get_model_evaluation]
        // [END automl_translate_get_model_evaluation]
        // [END automl_vision_classification_get_model_evaluation]
        // [END automl_vision_object_detection_get_model_evaluation]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((GetModelEvaluationOptions opts) =>
                AutoMLGetModelEvaluation.GetModelEvaluation(
                    opts.ProjectID,
                    opts.ModelId,
                    opts.ModelEvalId));
        }
    }
}
