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

// [START automl_delete_dataset]

using Google.Cloud.AutoML.V1;
using Google.Protobuf.WellKnownTypes;
using System;

class AutoMLDeleteDataset
{
    // TODO: complete summary
    public static void DeleteDataset(string projectId = "YOUR-PROJECT-ID",
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
    }
}

// [END automl_delete_dataset]