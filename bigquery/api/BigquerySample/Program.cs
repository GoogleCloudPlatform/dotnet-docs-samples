/*
 * Copyright (c) 2016 Google Inc.
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
// [START bigquery_simple_app_all]
// [START bigquery_simple_app_deps]

using System;
using Google.Cloud.BigQuery.V2;
// [START bigquery_simple_app_deps]

namespace GoogleCloudSamples
{
    public class BigquerySample
    {
        const string usage = @"Usage:
BigquerySample <project_id>";

        private static void Main(string[] args)
        {
            string projectId = null;
            if (args.Length == 0)
            {
                Console.WriteLine(usage);
            }
            else
            {
                projectId = args[0];
                // [START bigquery_simple_app_client]
                // By default, the Google.Cloud.BigQuery.V2 library client will authenticate 
                // using the service account file (created in the Google Developers 
                // Console) specified by the GOOGLE_APPLICATION_CREDENTIALS 
                // environment variable. If you are running on
                // a Google Compute Engine VM, authentication is completely 
                // automatic.
                var client = BigQueryClient.Create(projectId);
                // [END bigquery_simple_app_client]
                // [START bigquery_simple_app_query]
                string query = @"SELECT
                    CONCAT(
                        'https://stackoverflow.com/questions/',
                        CAST(id as STRING)) as url, view_count
                    FROM `bigquery-public-data.stackoverflow.posts_questions`
                    WHERE tags like '%google-bigquery%'
                    ORDER BY view_count DESC
                    LIMIT 10";
                var result = client.ExecuteQuery(query, parameters: null);
                // [END bigquery_simple_app_query]
                // [START bigquery_simple_app_print]
                Console.Write("\nQuery Results:\n------------\n");
                foreach (var row in result)
                {
                    Console.WriteLine($"{row["url"]}: {row["view_count"]} views");
                }
                // [END bigquery_simple_app_print]
            }
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }
    }
}
// [END bigquery_simple_app_all]
