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

// [START automl_export_dataset]

using Google.Cloud.AutoML.V1;
using Google.Protobuf.WellKnownTypes;
using System;

class AutoMLExportDataset
{
    // TODO: complete summary
    public static void ExportDataset(string projectId = "YOUR-PROJECT-ID",
        string datasetId = "YOUR-DATASET-ID", string gcsUri = "gs://BUCKET_ID/path_to_export/")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests. After completing all of your requests, call
        // the "close" method on the client to safely clean up any remaining background resources.
        AutoMlClient client = AutoMlClient.Create();

        // Get the complete path of the dataset.
        string datasetFullPath = DatasetName.Format(projectId, "us-central1", datasetId);
        DatasetName datasetFullId = DatasetName.Parse(datasetFullPath);
        GcsDestination gcsDestination = new GcsDestination
        {
            OutputUriPrefix = gcsUri
        };

        // Export the dataset to the output URI.
        OutputConfig outputConfig = new OutputConfig
        {
            GcsDestination = gcsDestination
        };

        Console.WriteLine("Processing export...");
        Empty response = client.ExportDataAsync(datasetFullId, outputConfig).Result.PollUntilCompleted().Result;
        Console.WriteLine($"Dataset exported. {response}");
    }
}

// [END automl_export_dataset]