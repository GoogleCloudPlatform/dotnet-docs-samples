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
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    [Verb("delete_dataset", HelpText = "Delete a dataset")]
<<<<<<< HEAD
    public class DeleteDatasetOptions : BaseOptions
=======
    public class AutoMLDeleteDatasetOptions : BaseOptions
>>>>>>> d42fde96... finished IT
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
            // Initialize the client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests.
            AutoMlClient client = AutoMlClient.Create();

            // Get the full path of the dataset.
            DatasetName datasetFullId = new DatasetName(projectId, "us-central1", datasetId);
            var result = Task.Run(() => client.DeleteDatasetAsync(datasetFullId)).Result;
            result.PollUntilCompleted();
            Console.WriteLine("Dataset deleted.");

            return 0;
        }
        // [END automl_delete_dataset]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((DeleteDatasetOptions opts) =>
                AutoMLDeleteDataset.DeleteDataset(opts.ProjectID, opts.DatasetId));
        }
    }
}
