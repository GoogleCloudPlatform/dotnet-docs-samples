// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START bigquery_export_model]

using Google.Cloud.BigQuery.V2;
using System;

public class BigQueryExtractModel
{
    public void ExtractModel(string projectId, string datasetId, string modelId, string destinationUri)
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        BigQueryJob job = client.CreateModelExtractJob(
            projectId: projectId,
            datasetId: datasetId,
            modelId: modelId,
            destinationUri: destinationUri
        );
        job = job.PollUntilCompleted().ThrowOnAnyError();  // Waits for the job to complete.
        System.IO.File.AppendAllText("log.txt", $"Exported model to {destinationUri}");
        Console.Write($"Exported model to {destinationUri}");
    }
}
// [END bigquery_export_model]
