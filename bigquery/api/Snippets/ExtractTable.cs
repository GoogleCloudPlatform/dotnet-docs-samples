// Copyright(c) 2018 Google LLC
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
//
// [START bigquery_extract_table]
using Google.Cloud.BigQuery.V2;
using System;

public class BigQueryExtractTable
{
    public void ExtractTable(
        string projectId = "your-project-id",
        string bucketName = "your-bucket-name")
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        string destinationUri = $"gs://{bucketName}/shakespeare.csv";
        BigQueryJob job = client.CreateExtractJob(
            projectId: "bigquery-public-data",
            datasetId: "samples",
            tableId: "shakespeare",
            destinationUri: destinationUri
        );
        job.PollUntilCompleted();  // Waits for the job to complete.
        Console.Write($"Exported table to {destinationUri}.");
    }
}
// [END bigquery_extract_table]
