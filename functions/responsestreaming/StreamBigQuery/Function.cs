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

using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Google.Cloud.BigQuery.V2;
using System;

namespace StreamBigQuery;

public class Function : IHttpFunction
{
    public async Task HandleAsync(HttpContext context)
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        BigQueryClient client = BigQueryClient.Create(projectId).WithDefaultLocation(Locations.UnitedStates);
        // Example to retrieve a large payload from BigQuery public dataset
        string query = "SELECT abstract FROM `bigquery-public-data.breathe.bioasq` LIMIT 1000";
        
        BigQueryJob job = client.CreateQueryJob(
            sql: query,
            parameters: null,
            options: new QueryOptions { UseQueryCache = false });

        job = (await job.PollUntilCompletedAsync()).ThrowOnAnyError();
        // Stream out the payload response by iterating rows (payload chunked by row).
        foreach (BigQueryRow row in client.GetQueryResults(job.Reference))
        {
            await context.Response.WriteAsync($"{row["abstract"]}\n");
        }

    }
}

// [END functions_response_streaming]
