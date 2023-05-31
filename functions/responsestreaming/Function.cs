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
        string query = @"
            SELECT abstract FROM `bigquery-public-data.breathe.bioasq` LIMIT 10";
        
        BigQueryJob job = client.CreateQueryJob(
            sql: query,
            parameters: null,
            options: new QueryOptions { UseQueryCache = false });
        // Wait for the job to complete.
        job = job.PollUntilCompleted().ThrowOnAnyError();
        // Display the results
        foreach (BigQueryRow row in client.GetQueryResults(job.Reference))
        {
            await context.Response.WriteAsync($"{row["abstract"]}" + "\n");
        }

    }
}
