// Copyright(c) 2018 Google Inc.
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
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Dialogflow.V2;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.BigQuery
{
    /// <summary>
    /// Handler for all "bigquery.hackernews" DialogFlow intent.
    /// </summary>
    [Intent("bigquery.hackernews")]
    public class BigQueryHackernewsHandler : BaseBigQueryHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public BigQueryHackernewsHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the intent.
        /// </summary>
        /// <param name="req">Webhook request</param>
        /// <returns>Webhook response</returns>
        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            // Extract the DialogFlow date, without the time, that has been requested
            // Format is "yyyy-mm-dd"
            var date = req.QueryResult.Parameters.Fields["date"].StringValue;
            date = date.Substring(0, Math.Min(10, date.Length));

            // Create the BigQuery client with default credentials
            var bigQueryClient = await BigQueryClient.CreateAsync(Program.AppSettings.GoogleCloudSettings.ProjectId);

            // Build the parameterized SQL query
            var table = bigQueryClient.GetTable("bigquery-public-data", "hacker_news", "full");
            var sql = $"SELECT title, url \nFROM {table} \n" +
                "WHERE STARTS_WITH(CAST(timestamp AS STRING), @date) AND type=\"story\" \n" +
                "ORDER BY score DESC \n" +
                "LIMIT 10";

            // Create the BigQuery parameters
            var parameters = new[]
            {
                new BigQueryParameter("date", BigQueryDbType.String, date)
            };

            // Show SQL query in browser
            ShowQuery(sql, parameters);

            // Time the BigQuery execution with a StopWatch
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Execute BigQuery SQL query. This can take time
            var result = await bigQueryClient.ExecuteQueryAsync(sql, parameters);

            // Query finished, stop the StopWatch
            stopwatch.Stop();

            // Get a job reference, for statistics
            var job = await bigQueryClient.GetJobAsync(result.JobReference);

            // Get result list, and check that there are some results
            var resultList = result.ToList();
            if (resultList.Count == 0)
            {
                return new WebhookResponse 
                {
                    FulfillmentText = "Sorry, there is no data for that date."
                };
            }

            // Time and data statistics
            long processedMb = job.Statistics.TotalBytesProcessed.Value / (1024 * 1024);
            double secs = stopwatch.Elapsed.TotalSeconds;
            var titles = resultList.Select(x => x["title"].ToString()).ToList();

            // Show SQL query and query results in browser
            ShowQuery(sql, parameters, (processedMb, secs, titles));

            // Send spoken response to DialogFlow
            return new WebhookResponse 
            {
                FulfillmentText = $"Scanned {processedMb} mega-bytes in {secs:0.0} seconds. " +
                $"The top title on hacker news was titled: {titles.First()}"
            };
        }
    }
}
