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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.BigQuery
{
    /// <summary>
    /// Base class for BigQuery related intent handlers. 
    /// </summary>
    public abstract class BaseBigQueryHandler : BaseHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public BaseBigQueryHandler(Conversation conversation) : base(conversation)
        {
        }

        protected async Task<(List<BigQueryRow>, long, double)> RunQueryAsync(BigQueryClient bigQueryClient, string sql, BigQueryParameter[] parameters)
        {
            // Show SQL query in browser
            ShowQuery(sql, parameters);

            // Time the BigQuery execution with a StopWatch
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Execute BigQuery SQL query.
            var result = await bigQueryClient.ExecuteQueryAsync(sql, parameters);

            // Query finished, stop the StopWatch
            stopwatch.Stop();

           // Get a job reference, for statistics
            var job = await bigQueryClient.GetJobAsync(result.JobReference);

            // Get result list
            var resultList = result.ToList();

            // Time and data statistics
            long processedMb = job.Statistics.TotalBytesProcessed.Value / (1024 * 1024);
            double secs = stopwatch.Elapsed.TotalSeconds;

            return (resultList, processedMb, secs);
        }

        protected void ShowQuery(string sql, IEnumerable<BigQueryParameter> parameters,
            (long processedMb, double secs, List<string> titles)? results = null)
        {
            var toShow = $"<div>{sql.Replace("\n", "<br/>")}</div>" +
                $"<div>Parameters: {string.Join("; ", parameters.Select(x => $"{x.Name}='{x.Value}'"))}</div>";

            if (results != null)
            {
                var (processedMb, secs, titles) = results.Value;
                toShow += $"<div>Scanned {processedMb}Mb in {secs:0.0} seconds.</div>";
                toShow += "<div><ol>" + string.Join("", titles.Select(x => $"<li>{x}</li>")) + "</ol></div>";
            }

            DialogflowApp.Show(toShow);
        }
    }
}
