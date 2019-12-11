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
using Google.Protobuf;
using System;
using System.IO;

namespace GoogleCloudSamples
{
    [Verb("vision_object_detection_predict", HelpText = "Prediction for an image (object detection)")]
    public class VisionObjectDetectionPredictOptions : PredictOptions
    {
        [Value(2, HelpText = "Location of image file.")]
        public string FilePath { get; set; }
    }

    class AutoMLVisionObjectDetectionPredict
    {
        // [START automl_vision_object_detection_predict]
        /// <summary>
        /// Demonstrates using the AutoML client to predict the text content using given model.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="modelId">the Id of the model.</param>
        /// <param name="filePath">the Local text file path of the content to be classified.</param>
        public static object VisionObjectDetectionPredict(string projectId = "YOUR-PROJECT-ID",
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


            Image image = new Image
            {
                ImageBytes = content
            };
            ExamplePayload payload = new ExamplePayload
            {
                Image = image
            };


            PredictRequest predictRequest = new
                PredictRequest
            {
                Name = modelFullId,
                Payload = payload,
                Params = {
                { "score_threshold", "0.5" } // [0.0-1.0] Only produce results higher than this value
            }
            };

            PredictResponse response = client.Predict(predictRequest);

            foreach (AnnotationPayload annotationPayload in response.Payload)
            {
                Console.WriteLine($"Predicted class name: {annotationPayload.DisplayName}");
                Console.WriteLine(
                    $"Predicted sentiment score: " +
                    $"{annotationPayload.ImageObjectDetection.Score.ToString("0.00")}");
                BoundingPoly boundingPoly = annotationPayload.ImageObjectDetection.BoundingBox;
                Console.WriteLine("Normalized Vertices:");
                foreach (NormalizedVertex vertex in boundingPoly.NormalizedVertices)
                {
                    Console.WriteLine($"\tX: {vertex.X}, Y: {vertex.Y}");
                }
            }
            return 0;
        }
        // [END automl_vision_object_detection_predict]
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((VisionObjectDetectionPredictOptions opts) =>
                     AutoMLVisionObjectDetectionPredict.VisionObjectDetectionPredict(opts.ProjectID,
                                                                 opts.ModelID,
                                                                 opts.FilePath));
        }
    }
}
