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
// [START bigquery_quickstart]

using System;
using Google.Bigquery.V2;

namespace GoogleCloudSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            // Your Google Cloud Platform project ID
            string projectId = "YOUR_PROJECT_ID";

            // Instantiates a client
            BigqueryClient client = BigqueryClient.Create(projectId);

            // The id for the new dataset
            string datasetId = "my_new_dataset";

            // Creates the dataset
            BigqueryDataset dataset = client.CreateDataset(datasetId);

            Console.WriteLine($"Dataset {dataset.FullyQualifiedId} created.");
        }
    }
}
// [END bigquery_quickstart]
