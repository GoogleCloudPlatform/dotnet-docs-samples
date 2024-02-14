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

// [START bigquery_create_table_range_partitioned]

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;

public class BigQueryCreateTableRangePartitioned
{
    public BigQueryTable CreateTable(string projectId, string datasetId, string tableId)
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        var dataset = client.GetDataset(datasetId);

        // Note: The field must be a top- level, NULLABLE/REQUIRED field.
        // The only supported type is INTEGER/INT64.
        var partitioning = new RangePartitioning
        {
            Field = "integerField",
            Range = new RangePartitioning.RangeData
            {
                Start = 1,
                Interval = 2,
                End = 10
            }
        };
        var schema = new TableSchemaBuilder
        {
            { "integerField", BigQueryDbType.Int64 },
            { "stringField", BigQueryDbType.String },
            { "booleanField", BigQueryDbType.Bool },
            { "dateField", BigQueryDbType.Date }
        }.Build();

        var table = new Table
        {
            RangePartitioning = partitioning,
            Schema = schema
        };
        return dataset.CreateTable(tableId, table);
    }
}
// [END bigquery_create_table_range_partitioned]
