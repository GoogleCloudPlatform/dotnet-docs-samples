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
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    [Verb("batch_predict", HelpText = "Batch Predict your files and save the results to GCS")]
    public class BatchPredictOptions : PredictOptions
    {
        [Value(2, HelpText = "GCS bucket of your csv or jsonl file that contains paths to the images or text contents.")]
        public string InputUri { get; set; }

        [Value(3, HelpText = "GCS location to save the results.")]
        public string OutputUri { get; set; }
    }
    class AutoMLBatchPredict
    {
        // [START automl_batch_predict]
        /// <summary>
        /// Demonstrates using the AutoML client to predict image or text contents.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="modelId">the Id of the model.</param>
        /// <param name="inputUri">GCS bucket of your csv or jsonl file that contains paths to the images or text contents.</param>
        /// <param name="outputUri">The GCS path to store the output of your prediction request.</param>
        public static object BatchPredict(string projectId = "YOUR-PROJECT-ID",
            string modelId = "YOUR-MODEL-ID",
            string inputUri = "gs://YOUR_BUCKET_ID/path_to_your_input_csv_or_jsonl", //images or text contents
            string outputUri = "gs://YOUR_BUCKET_ID/path_to_save_results/")
        {
            // Initialize the client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests.
            PredictionServiceClient client = PredictionServiceClient.Create();

            // Get the full path of the model.
            string modelFullId = ModelName.Format(projectId, "us-central1", modelId);

            GcsSource gcsSource = new GcsSource
            {
                InputUris = { inputUri }
            };

            BatchPredictInputConfig inputConfig = new BatchPredictInputConfig
            {
                GcsSource = gcsSource
            };

            GcsDestination gcsDestination = new GcsDestination
            {
                OutputUriPrefix = outputUri
            };

            BatchPredictOutputConfig outputConfig = new BatchPredictOutputConfig
            {
                GcsDestination = gcsDestination
            };

            BatchPredictRequest request = new BatchPredictRequest
            {
                Name = modelFullId,
                InputConfig = inputConfig,
                OutputConfig = outputConfig
            };

            var result = Task.Run(() => client.BatchPredictAsync(request)).Result;
            Console.WriteLine("Waiting for operation to complete...");
            result.PollUntilCompleted();

            Console.WriteLine("Batch Prediction results saved to specified Cloud Storage bucket.");
            return 0;
        }
        // [END automl_batch_predict]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((BatchPredictOptions opts) => AutoMLBatchPredict.BatchPredict(
                         opts.ProjectID,
                         opts.ModelID,
                         opts.InputUri,
                         opts.OutputUri));
        }
    }
}
