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

// [START automl_get_model_evaluation]

using Google.Cloud.AutoML.V1;
using System;

class AutoMLGetModelEvaluation
{
    /// <summary>
    /// Demonstrates using the AutoML client to get model evaluations.
    /// </summary>
    /// <param name="projectId">GCP Project ID.</param>
    /// <param name="modelId">the Id of the model.</param>
    /// <param name="modelEvaluationId">the Id of your model evaluation.</param>
    public static void GetModelEvaluation(string projectId = "YOUR-PROJECT-ID",
        string modelId = "YOUR-MODEL-ID",
        string modelEvaluationId = " YOUR-MODEL-EVAL-ID")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests. After completing all of your requests, call
        // the "close" method on the client to safely clean up any remaining background resources.
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
        Console.WriteLine(
            $"Model Evaluation Metrics: {modelEvaluation.TranslationEvaluationMetrics}");
    }
}

// [END automl_get_model_evaluation]