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

// [START automl_vision_classification_predict]

using Google.Cloud.AutoML.V1;
using Google.Protobuf;
using System;
using System.IO;

class AutoMLVisionClassificationPredict
{
    // TODO: complete summary
    public static void VisionClassificationPredict(string projectId = "YOUR-PROJECT-ID",
        string modelId = "YOUR-MODEL-ID",
        string filePath = "path_to_local_file.jpg")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests. After completing all of your requests, call
        // the "close" method on the client to safely clean up any remaining background resources.
        PredictionServiceClient client = PredictionServiceClient.Create();

        // Get the full path of the model.
        string modelFullId = ModelName.Format(projectId, "us-central1", modelId);
        ByteString content = ByteString.CopyFrom(File.ReadAllBytes(filePath));


        Image image = new Image {
            ImageBytes = content
        };
        ExamplePayload payload = new ExamplePayload {
            Image = image
        };


        PredictRequest predictRequest = new
            PredictRequest
        {
            Name = modelFullId,
            Payload = payload,
            Params = {
                { "score_threshold", "0.8" } // [0.0-1.0] Only produce results higher than this value
            }
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

// [END automl_vision_classification_predict]