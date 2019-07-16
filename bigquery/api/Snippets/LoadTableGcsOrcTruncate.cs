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
// [START bigquery_load_table_gcs_orc_truncate]

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using System;

public class BigQueryLoadTableGcsOrcTruncate
{
    public void LoadTableGcsOrcTruncate(
        string projectId = "your-project-id",
        string datasetId = "your_dataset_id",
        string tableId = "your_table_id"
    )
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        var gcsURI = "gs://cloud-samples-data/bigquery/us-states/us-states.orc";
        var dataset = client.GetDataset(datasetId);
        TableReference destinationTableRef = dataset.GetTableReference(
            tableId: "us_states");
        // Create job configuration
        var jobOptions = new CreateLoadJobOptions()
        {
            SourceFormat = FileFormat.Orc,
            WriteDisposition = WriteDisposition.WriteTruncate
        };
        // Create and run job
        var loadJob = client.CreateLoadJob(
            sourceUri: gcsURI,
            destination: destinationTableRef,
            // Pass null as the schema because the schema is inferred when
            // loading Orc data
            schema: null, options: jobOptions);
        loadJob = loadJob.PollUntilCompleted().ThrowOnAnyError();  // Waits for the job to complete.
        // Display the number of rows uploaded
        BigQueryTable table = client.GetTable(destinationTableRef);
        Console.WriteLine(
            $"Loaded {table.Resource.NumRows} rows to {table.FullyQualifiedId}");
    }
}
// [END bigquery_load_table_gcs_orc_truncate]
