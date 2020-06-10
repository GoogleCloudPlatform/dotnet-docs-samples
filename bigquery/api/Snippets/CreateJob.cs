// Copyright(c) 2020 Google LLC
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
// [START bigquery_create_job]

using Google.Cloud.BigQuery.V2;
using System;
using System.Collections.Generic;

public class BigQueryCreateJob
{
    public BigQueryJob CreateJob(string projectId = "your-project-id")
    {
        string query = @"
            SELECT country_name from `bigquery-public-data.utility_us.country_code_iso";

        // Initialize client that will be used to send requests.
        BigQueryClient client = BigQueryClient.Create(projectId);

        QueryOptions queryOptions = new QueryOptions
        {
            JobLocation = "us",
            JobIdPrefix = "code_sample_",
            Labels = new Dictionary<string, string>
            {
                ["example-label"] = "example-value"
            },
            MaximumBytesBilled = 1000000
        };

        BigQueryJob queryJob = client.CreateQueryJob(
            sql: query,
            parameters: null,
            options: queryOptions);

        Console.WriteLine($"Started job: {queryJob.Reference.JobId}");
        return queryJob;
    }
}
// [END bigquery_create_job]
