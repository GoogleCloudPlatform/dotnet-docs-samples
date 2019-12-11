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
    [Verb("language_batch_predict", HelpText = "Make predictions for multiple text contents.")]
    public class LanguageBatchPredictOptions : BaseOptions
    {
        [Value(1, HelpText = "ID of model for AutoML Language.")]
        public string ModelId { get; set; }

        [Value(2, HelpText = "GCS bucket location for input file.")]
        public string InputUri { get; set; }

        [Value(3, HelpText = "GCS bucket path to save the result.")]
        public string OutputUri { get; set; }
    }
    class AutoMLLanguageBatchPredict
    {
        // [START automl_language_batch_predict]
        /// <summary>
        /// Demonstrates using the AutoML client to classify the text content.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="inputUri">The GCS path where input file is stored.</param>
        /// <param name="outputUri">The GCS path for classified output.</param>
        public static object LanguageBatchPredict(string projectId = "YOUR-PROJECT-ID",
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
                OutputConfig = outputConfig
            };

            client.BatchPredictAsync(request).Result.PollUntilCompleted();

            Console.WriteLine("Waiting for operation to complete...");
            Console.WriteLine("Batch Prediction results saved to specified Cloud Storage bucket.");
            return 0;
        }

        // [END automl_language_batch_predict]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((LanguageBatchPredictOptions opts) =>
                     AutoMLLanguageBatchPredict.LanguageBatchPredict(opts.ProjectID,
                                                                opts.ModelId,
                                                                 opts.InputUri,
                                                                 opts.OutputUri));
        }
    }
}
