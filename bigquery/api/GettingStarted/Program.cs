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
// [START complete]

using System;
using Google.Bigquery.V2;

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
                // [START setup]
                // By default, the Google.Bigquery.V2 library client will authenticate 
                // using the service account file (created in the Google Developers 
                // Console) specified by the GOOGLE_APPLICATION_CREDENTIALS 
                // environment variable. If you are running on
                // a Google Compute Engine VM, authentication is completely 
                // automatic.
                var client = BigqueryClient.Create(projectId);
                // [END setup]
                // [START query]
                var table = client.GetTable("bigquery-public-data", "samples", "shakespeare");

                string query = $@"SELECT corpus AS title, COUNT(*) AS unique_words FROM `{table.FullyQualifiedId}` 
                    GROUP BY title ORDER BY unique_words DESC LIMIT 42";
                var result = client.ExecuteQuery(query);
                // [END query]
                // [START print_results]
                Console.Write("\nQuery Results:\n------------\n");
                foreach (var row in result.GetRows())
                {
                    Console.WriteLine($"{row["title"]}: {row["unique_words"]}");
                }
                // [END print_results]
            }
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }
    }
}
// [END complete]
