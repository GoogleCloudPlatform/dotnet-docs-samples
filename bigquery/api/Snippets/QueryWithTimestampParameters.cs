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
// [START bigquery_query_params_timestamps]

using Google.Cloud.BigQuery.V2;
using System;

public class BigQueryQueryWithTimestampParameters
{
    public void QueryWithTimestampParameters(string projectId = "project-id")
    {
        var timestamp = new DateTime(2016, 12, 7, 8, 0, 0, DateTimeKind.Utc);

        // Note: Standard SQL is required to use query parameters.
        var query = "SELECT TIMESTAMP_ADD(@ts_value, INTERVAL 1 HOUR);";

        // Initialize client that will be used to send requests.
        var client = BigQueryClient.Create(projectId);

        var parameters = new BigQueryParameter[]
        {
            new BigQueryParameter("ts_value", BigQueryDbType.Timestamp, timestamp),
        };

        var job = client.CreateQueryJob(
            sql: query,
            parameters: parameters,
            options: new QueryOptions { UseQueryCache = false });
        // Wait for the job to complete.
        job = job.PollUntilCompleted().ThrowOnAnyError();
        // Display the results
        foreach (BigQueryRow row in client.GetQueryResults(job.Reference))
        {
            Console.WriteLine(row[0]);
        }
    }
}
// [END bigquery_query_params_timestamps]
