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
// [START bigquery_delete_dataset_and_contents]

using Google.Cloud.BigQuery.V2;
using System;

public class BigQueryDeleteDatasetAndContents
{
    public void DeleteDatasetAndContents(
        string projectId = "your-project-id",
        string datasetId = "your_dataset_with_tables"
    )
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        // Use the DeleteDatasetOptions to delete a dataset and its contents
        client.DeleteDataset(
            datasetId: datasetId,
            options: new DeleteDatasetOptions() { DeleteContents = true }
        );
        Console.WriteLine($"Dataset {datasetId} and contents deleted.");
    }
}
// [END bigquery_delete_dataset_and_contents]
