// Copyright(c) 2018 Google Inc.
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
// [START bigquery_load_table_gcs_csv]
using Google.Cloud.BigQuery.V2;
using System;

public class BigQueryLoadTableGcsCsv
{
    public BigQueryTable LoadTableGcsCsv(
        string projectId = "your-project-id",
        string datasetId = "your_dataset_id"
    )
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        var gcsURI = "gs://cloud-samples-data/bigquery/us-states/us-states.csv";
        var dataset = client.GetDataset(datasetId);
        var schema = new TableSchemaBuilder {
            { "name", BigQueryDbType.String },
            { "post_abbr", BigQueryDbType.String }
        }.Build();
        var jobOptions = new CreateLoadJobOptions()
        {
            // The source format defaults to CSV; line below is optional.
            SourceFormat = FileFormat.Csv,
            SkipLeadingRows = 1
        };
        var destinationTable = dataset.GetTableReference(
            tableId: "us_states");
        var loadJob = client.CreateLoadJob(
            sourceUri: gcsURI, destination: destinationTable,
            schema: schema, options: jobOptions);
        loadJob.PollUntilCompleted();
        return client.GetTable(destinationTable);
    }
}
// [END bigquery_load_table_gcs_csv]
