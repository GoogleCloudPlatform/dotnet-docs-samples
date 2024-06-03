// Copyright 2023 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START functions_response_streaming]

using Google.Cloud.BigQuery.V2;
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace StreamBigQuery;

public class Function : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        BigQueryClient client = BigQueryClient.Create(projectId).WithDefaultLocation(Locations.UnitedStates);

        // Example to retrieve a large payload from BigQuery public dataset.
        string query = "SELECT title FROM `bigquery-public-data.breathe.bioasq` LIMIT 1000";
        BigQueryParameter[] parameters = null;
        BigQueryResults results = await client.ExecuteQueryAsync(query, parameters, cancellationToken: context.RequestAborted);

        // Stream out the payload response by iterating rows.
        foreach (BigQueryRow row in results)
        {
            await context.Response.WriteAsync($"{row["title"]}\n", context.RequestAborted);
            // Artificially add a 50ms delay per line, just to make the streaming nature clearer
            // when viewing the results in a browser.
            await Task.Delay(50);
        }
    }
}
// [END functions_response_streaming]
