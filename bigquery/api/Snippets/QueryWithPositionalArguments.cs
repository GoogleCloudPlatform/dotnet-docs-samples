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
// [START bigquery_query_params_positional]

using Google.Cloud.BigQuery.V2;
using System;

public class BigQueryQueryWithPositionalParameters
{
    public void QueryWithPositionalParameters(string projectId = "project-id")
    {
        var corpus = "romeoandjuliet";
        var minWordCount = 250;

        // Note: Standard SQL is required to use query parameters.
        var query = @"
                SELECT word, word_count
                FROM `bigquery-public-data.samples.shakespeare`
                WHERE corpus = ?
                AND word_count >= ?
                ORDER BY word_count DESC;";

        // Initialize client that will be used to send requests.
        var client = BigQueryClient.Create(projectId);

        // Set the name to None to use positional parameters.
        // Note that you cannot mix named and positional parameters.
        var parameters = new BigQueryParameter[]
        {
            new BigQueryParameter(null, BigQueryDbType.String, corpus),
            new BigQueryParameter(null, BigQueryDbType.Int64, minWordCount)
        };

        var job = client.CreateQueryJob(
            sql: query,
            parameters: parameters,
            options: new QueryOptions
            {
                UseQueryCache = false,
                ParameterMode = BigQueryParameterMode.Positional
            });
        // Wait for the job to complete.
        job = job.PollUntilCompleted().ThrowOnAnyError();
        // Display the results
        foreach (BigQueryRow row in client.GetQueryResults(job.Reference))
        {
            Console.WriteLine($"{row["word"]}: {row["word_count"]}");
        }
    }
}
// [END bigquery_query_params_positional]
