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

using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using System;

public class BigQueryAuthenticateServiceAccount
{
    public void AuthenticateWithServiceAccount(string projectId,
                                               string jsonPath)
    {
        // [START bigquery_client_json_credentials]
        var credentials = GoogleCredential.FromFile(jsonPath);
        var client = BigQueryClient.Create(projectId, credentials);
        // [END bigquery_client_json_credentials]

        // Use the client
        string query = @"
            SELECT name
            FROM `bigquery-public-data.usa_names.usa_1910_2013`
            WHERE state = 'TX'
            LIMIT 100";
        BigQueryJob job = client.CreateQueryJob(
            sql: query,
            parameters: null,
            options: new QueryOptions { UseQueryCache = false });
        // Wait for the job to complete.
        job = job.PollUntilCompleted().ThrowOnAnyError();
        // Display the results
        foreach (BigQueryRow row in client.GetQueryResults(job.Reference))
        {
            Console.WriteLine($"{row["name"]}");
        }
    }
}
