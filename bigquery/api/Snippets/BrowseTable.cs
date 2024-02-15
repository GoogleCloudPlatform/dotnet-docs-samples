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
// [START bigquery_browse_table]

using Google.Api.Gax;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using System;
using System.Linq;

public class BigQueryBrowseTable
{
    public void BrowseTable(
        string projectId = "your-project-id"
    )
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        TableReference tableReference = new TableReference()
        {
            TableId = "shakespeare",
            DatasetId = "samples",
            ProjectId = "bigquery-public-data"
        };
        // Load all rows from a table
        PagedEnumerable<TableDataList, BigQueryRow> result = client.ListRows(
            tableReference: tableReference,
            schema: null
        );
        // Print the first 10 rows
        foreach (BigQueryRow row in result.Take(10))
        {
            Console.WriteLine($"{row["corpus"]}: {row["word_count"]}");
        }
    }
}
// [END bigquery_browse_table]
