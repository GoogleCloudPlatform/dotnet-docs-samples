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
    [Verb("vision_batch_predict", HelpText = "Multiple predictions for images using a batch predict.")]
    public class VisionBatchPredictOptions : PredictOptions
    {
        [Value(2, HelpText = "Input GCS location that contains the paths to the images to annotate")]
        public string InputUri { get; set; }

        [Value(3, HelpText = "GCS path to save the result")]
        public string OutputUri { get; set; }
    }
    class AutoMLVisionBatchPredict
    {
        // [START automl_vision_batch_predict]
        /// <summary>
        /// Demonstrates using the AutoML client to predict image contents.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="modelId">the Id of the model.</param>
        /// <param name="inputUri">The GCS path where all images are.</param>
        /// <param name="outputUri">The GCS path to store the output of your prediction request.</param>
        public static object VisionBatchPredict(string projectId = "YOUR-PROJECT-ID",
            string modelId = "YOUR-MODEL-ID",
            String inputUri = "gs://YOUR_BUCKET_ID/path_to_your_input_file.json",
            String outputUri = "gs://YOUR_BUCKET_ID/path_to_save_results/")
        {
            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            PredictionServiceClient client = PredictionServiceClient.Create();

            // Get the full path of the model.
            string modelFullId = ModelName.Format(projectId, "us-central1", modelId);

            GcsSource gcsSource = new GcsSource
            {
                InputUris = { inputUri }
            };

            BatchPredictInputConfig inputConfig = new
                BatchPredictInputConfig
            {
                GcsSource = gcsSource
            };

            GcsDestination gcsDestination = new
                GcsDestination
            {
                OutputUriPrefix = outputUri
            };

            BatchPredictOutputConfig outputConfig = new
                BatchPredictOutputConfig
            {
                GcsDestination = gcsDestination
            };

            BatchPredictRequest request = new
                BatchPredictRequest
            {
                Name = modelFullId,
                InputConfig = inputConfig,
                OutputConfig = outputConfig,
                Params =
            {
                { "score_threshold" , "0.8" } // [0.0-1.0] Only produce results higher than this value
            }
            };

            client.BatchPredictAsync(request).Result.PollUntilCompleted();

            Console.WriteLine("Waiting for operation to complete...");
            Console.WriteLine("Batch Prediction results saved to specified Cloud Storage bucket.");
            return 0;
        }
        // [END automl_vision_batch_predict]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((VisionBatchPredictOptions opts) =>
                     AutoMLVisionBatchPredict.VisionBatchPredict(opts.ProjectID,
                                                                 opts.ModelID,
                                                                 opts.InputUri,
                                                                 opts.OutputUri));
        }
    }
}
