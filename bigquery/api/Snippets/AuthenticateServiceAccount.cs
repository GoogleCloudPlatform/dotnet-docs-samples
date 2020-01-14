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
            SELECT name FROM `bigquery-public-data.usa_names.usa_1910_2013`
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
