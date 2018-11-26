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
// [START bigquery_list_datasets]

using Google.Cloud.BigQuery.V2;
using System;
using System.Collections.Generic;
using System.Linq;

public class BigQueryListDatasets
{
    public void ListDatasets(
        string projectId = "your-project-id"
    )
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        // Retrieve list of datasets in project
        List<BigQueryDataset> datasets = client.ListDatasets().ToList();
        // Display the results
        if (datasets.Count > 0)
        {
            Console.WriteLine($"Datasets in project {projectId}:");
            foreach (var dataset in datasets)
            {
                Console.WriteLine($"\t{dataset.Reference.DatasetId}");
            }
        }
        else
        {
            Console.WriteLine($"{projectId} does not contain any datasets.");
        }
    }
}
// [END bigquery_list_datasets]
