/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */
// [START all]

using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2;
using Google.Apis.Bigquery.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    public class BigquerySample
    {
        const string usage = @"Usage:
BigquerySample <project_id>
";
        // [START build_service]
        /// <summary>
        /// Creates an authorized Bigquery client service using Application
        /// Default Credentials.
        /// </summary>
        /// <returns>an authorized Bigquery client</returns>
        public BigqueryService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Bigquery scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    BigqueryService.Scope.Bigquery
                });
            }
            return new BigqueryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DotNet Bigquery Samples",
            });
        }
        // [END build_service]

        // [START run_query]
        /// <summary>
        /// Executes the given query synchronously.
        /// </summary>
        /// <param name="querySql">the query to execute.</param>
        /// <param name="bigquery">the BigquerService object.</param>
        /// <param name="projectId">the id of the project under which to run the
        /// query.</param>
        /// <returns>a list of the results of the query.</returns>
        public IList<TableRow> ExecuteQuery(string querySql,
            BigqueryService bigquery, string projectId)
        {
            var request = new Google.Apis.Bigquery.v2.JobsResource.QueryRequest(
                bigquery, new Google.Apis.Bigquery.v2.Data.QueryRequest()
                {
                    Query = querySql,
                }, projectId);
            var query = request.Execute();
            GetQueryResultsResponse queryResult = bigquery.Jobs.GetQueryResults(
                projectId, query.JobReference.JobId).Execute();
            return queryResult.Rows;
        }
        // [END run_query]

        // [START print_results]
        /// <summary>Prints the results to standard out.</summary>
        public void PrintResults(IList<TableRow> rows)
        {
            Console.Write("\nQuery Results:\n------------\n");
            foreach (TableRow row in rows)
            {
                foreach (TableCell field in row.F)
                {
                    Console.Write(String.Format("{0,-50}", field.V));
                }
                Console.WriteLine();
            }
        }
        // [END print_results]

        private static void Main(string[] args)
        {
            BigquerySample sample = new BigquerySample();
            string projectId = null;
            if (args.Length == 0)
            {
                Console.WriteLine(usage);
            }
            else
            {
                projectId = args[0];
                // Create a new Bigquery client authorized via Application Default 
                // Credentials.
                BigqueryService bigquery = sample.CreateAuthorizedClient();

                IList<TableRow> rows = sample.ExecuteQuery(
                    "SELECT TOP(corpus, 10) as title, COUNT(*) as unique_words " +
                    "FROM [publicdata:samples.shakespeare]", bigquery, projectId);

                sample.PrintResults(rows);
            }
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }
    }
}
// [END all]
