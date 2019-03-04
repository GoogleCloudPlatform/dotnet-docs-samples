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
// [START bigquery_list_tables]

using Google.Cloud.BigQuery.V2;
using System;
using System.Collections.Generic;
using System.Linq;

public class BigQueryListTables
{
    public void ListTables(
        string projectId = "your-project-id",
        string datasetId = "your_dataset_id"
    )
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        // Retrieve list of tables in the dataset
        List<BigQueryTable> tables = client.ListTables(datasetId).ToList();
        // Display the results
        if (tables.Count > 0)
        {
            Console.WriteLine($"Tables in dataset {datasetId}:");
            foreach (var table in tables)
            {
                Console.WriteLine($"\t{table.Reference.TableId}");
            }
        }
        else
        {
            Console.WriteLine($"{datasetId} does not contain any tables.");
        }
    }
}
// [END bigquery_list_tables]
