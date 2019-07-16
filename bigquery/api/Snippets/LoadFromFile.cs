// Copyright(c) 2018 Google LLC
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
//
// [START bigquery_load_from_file]

using Google.Cloud.BigQuery.V2;
using System;
using System.IO;

public class BigQueryLoadFromFile
{
    public void LoadFromFile(
        string projectId = "your-project-id",
        string datasetId = "your_dataset_id",
        string tableId = "your_table_id",
        string filePath = "path/to/file.csv"
    )
    {
        BigQueryClient client = BigQueryClient.Create(projectId);
        // Create job configuration
        var uploadCsvOptions = new UploadCsvOptions()
        {
            SkipLeadingRows = 1,  // Skips the file headers
            Autodetect = true
        };
        using (FileStream stream = File.Open(filePath, FileMode.Open))
        {
            // Create and run job
            // Note that there are methods available for formats other than CSV
            BigQueryJob job = client.UploadCsv(
                datasetId, tableId, null, stream, uploadCsvOptions);
            job = job.PollUntilCompleted().ThrowOnAnyError();  // Waits for the job to complete.

            // Display the number of rows uploaded
            BigQueryTable table = client.GetTable(datasetId, tableId);
            Console.WriteLine(
                $"Loaded {table.Resource.NumRows} rows to {table.FullyQualifiedId}");
        }
    }
}
// [END bigquery_load_from_file]
