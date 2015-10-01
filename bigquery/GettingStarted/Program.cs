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

namespace BQSample
{
    internal class Program
    {
        // [START build_service]
        /// <summary>
        /// Creates an authorized Bigquery client service using Application Default Credentials.
        /// </summary>
        /// <returns>an authorized Bigquery client</returns>
        private static async Task<BigqueryService> CreateAuthorizedClientAsync()
        {
            GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();
            // Inject the Bigquery scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[] { BigqueryService.Scope.Bigquery });
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
        /// <param name="projectId">the id of the project under which to run the query.</param>
        /// <returns>a list of the results of the query.</returns>
        private static async Task<IList<TableRow>> ExecuteQueryAsync(
            string querySql, BigqueryService bigquery, string projectId)
        {
            JobConfigurationQuery queryConfig = new JobConfigurationQuery { Query = querySql };
            JobConfiguration config = new JobConfiguration { Query = queryConfig };
            Job job = new Job { Configuration = config };
            JobsResource.InsertRequest insert = bigquery.Jobs.Insert(job, projectId);
            JobReference jobRef = (await insert.ExecuteAsync()).JobReference;
            GetQueryResultsResponse queryResult = await
                bigquery.Jobs.GetQueryResults(projectId, jobRef.JobId).ExecuteAsync();
            return queryResult.Rows;
        }
        // [END run_query]

        // [START print_results]
        /// <summary>Prints the results to standard out.</summary>
        private static void PrintResults(IList<TableRow> rows)
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
            string projectId = null;
            if (args.Length == 0)
            {
                // Prompt the user to enter the id of the project to run the queries under
                Console.Write("Enter the project ID: ");
                projectId = Console.ReadLine().Trim();
            }
            else
            {
                projectId = args[0];
            }
            // Create a new Bigquery client authorized via Application Default Credentials.
            BigqueryService bigquery = CreateAuthorizedClientAsync().Result;

            IList<TableRow> rows = ExecuteQueryAsync(
                "SELECT TOP(corpus, 10) as title, COUNT(*) as unique_words " +
                "FROM [publicdata:samples.shakespeare]", bigquery, projectId).Result;

            PrintResults(rows);
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
// [END all]
