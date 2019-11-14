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

// [START automl_language_text_classification_predict]

using Google.Cloud.AutoML.V1;
using System;

class AutoMLLanguageTextClassificationPredict
{
    // TODO: complete summary
    public static void LanguageTextClassificationPredict(string projectId = "YOUR-PROJECT-ID",
        string modelId = "YOUR-MODEL-ID",
        string content = "YOUR TEXT TO PREDICT")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests. After completing all of your requests, call
        // the "close" method on the client to safely clean up any remaining background resources.
        PredictionServiceClient client = PredictionServiceClient.Create();

        // Get the full path of the model.
        string modelFullId = ModelName.Format(projectId, "us-central1", modelId);

        TextSnippet textSnippet = new
            TextSnippet
        {
            Content = content,
            MimeType = "text/plain" // Types: text/plain, text/html
        };
        ExamplePayload payload = new ExamplePayload
        {
            TextSnippet = textSnippet
        };

        PredictRequest predictRequest = new
            PredictRequest
        {
            Name = modelFullId,
            Payload = payload
        };

        PredictResponse response = client.Predict(predictRequest);

        foreach (AnnotationPayload annotationPayload in response.Payload)
        {
            Console.WriteLine($"Predicted class name: {annotationPayload.DisplayName}");
            Console.WriteLine(
                $"Predicted sentiment score: " +
                $"{annotationPayload.Classification.Score.ToString("0.00")}");
        }
    }
}

// [END automl_language_text_classification_predict]