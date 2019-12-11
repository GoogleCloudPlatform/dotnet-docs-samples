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
    [Verb("import_dataset", HelpText = "Import a dataset")]
<<<<<<< HEAD
    public class ImportDatasetOptions : BaseOptions
=======
    public class AutoMLImportDatasetOptions : BaseOptions
>>>>>>> d42fde96... finished IT
    {
        [Value(1, HelpText = "the Id of the dataset.")]
        public string DatasetId { get; set; }

        [Value(2, HelpText = "GCS path to a training data.")]
        public string GcsPath { get; set; }
    }
    class AutoMLImportDataset
    {
        // [START automl_import_data]
        /// <summary>
        /// Import labeled items.
        /// </summary>
        /// <param name="projectId">GCP Project ID.</param>
        /// <param name="datasetId">the Id of the dataset.</param>
        /// <param name="path">Google Cloud Storage URIs. 
        /// Target files must be in AutoML CSV format.</param>
        public static object ImportDataset(string projectId = "YOUR-PROJECT-ID",
            string datasetId = "YOUR-DATASET-ID",
            string path = "gs://BUCKET_ID/path_to_training_data.csv")
        {
            // Initialize the client that will be used to send requests. This client only needs to be created
            // once, and can be reused for multiple requests.
            AutoMlClient client = AutoMlClient.Create();

            // Get the complete path of the dataset.
            string datasetFullId = DatasetName.Format(projectId, "us-central1", datasetId);

            // Get multiple Google Cloud Storage URIs to import data from
            GcsSource gcsSource = new GcsSource
            {
                InputUris = { path.Split(",") }
            };

            // Import data from the input URI
            InputConfig inputConfig = new InputConfig
            {
                GcsSource = gcsSource
            };

            var result = Task.Run(() => client.ImportDataAsync(datasetFullId, inputConfig)).Result;
            Console.WriteLine("Processing import...");
            result.PollUntilCompleted();
            Console.WriteLine($"Data imported.");
            return 0;
        }

        // [END automl_import_data]

        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((ImportDatasetOptions opts) =>
                     AutoMLImportDataset.ImportDataset(opts.ProjectID, opts.DatasetId, opts.GcsPath));
        }
    }
}
