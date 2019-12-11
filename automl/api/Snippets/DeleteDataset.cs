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
using Google.Protobuf.WellKnownTypes;
using System;

namespace GoogleCloudSamples
{
    [Verb("delete_dataset", HelpText = "Delete a dataset")]
    public class DeleteDatasetOptions : BaseOptions
    {
        [Value(1, HelpText = "Dataset ID to delete.")]
        public string DatasetId { get; set; }
    }
    class AutoMLDeleteDataset
    {
        // [START automl_delete_dataset]
        /// <summary>
        /// Deletes a dataset and all of its contents.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="datasetId">the Id of the dataset.</param>
        public static object DeleteDataset(string projectId = "YOUR-PROJECT-ID",
            string datasetId = "YOUR-DATASET-ID")
        {
            // Initialize client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests. After completing all of your requests, call
            // the "close" method on the client to safely clean up any remaining background resources.
            AutoMlClient client = AutoMlClient.Create();

            // Get the full path of the dataset.
            string datasetFullPath = DatasetName.Format(projectId, "us-central1", datasetId);
            DatasetName datasetFullId = DatasetName.Parse(datasetFullPath);
            Empty response = client.DeleteDatasetAsync(datasetFullId).Result.PollUntilCompleted().Result;
            Console.WriteLine($"Dataset deleted. {response}");

            return 0;
        }
        // [END automl_delete_dataset]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DeleteDatasetOptions opts) =>
                     AutoMLDeleteDataset.DeleteDataset(opts.ProjectID,
                                                                 opts.DatasetId));
        }
    }
}
