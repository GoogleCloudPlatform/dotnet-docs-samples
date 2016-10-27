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
// [START all]

using System;
// [START create_bigquery_client]
using Google.Bigquery.V2;
// [END create_bigquery_client]
using Google.Storage.V1;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.IO;
using System.Diagnostics;
using Google.Apis.Bigquery.v2.Data;

namespace GoogleCloudSamples
{
    public class BigqueryTest
    {
        private readonly string _projectId;
        private readonly BigqueryClient _client;

        public BigqueryTest()
        {
            // [START create_bigquery_client]
            // By default, the Google.Bigquery.V2 library client will authenticate 
            // using the service account file (created in the Google Developers 
            // Console) specified by the GOOGLE_APPLICATION_CREDENTIALS 
            // environment variable. If you are running on
            // a Google Compute Engine VM, authentication is completely 
            // automatic.
            _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _client = BigqueryClient.Create(_projectId);
            // [END create_bigquery_client]
        }

        // [START create_dataset]
        public void CreateDataset(string datasetId, BigqueryClient client)
        {
            var dataset = client.GetOrCreateDataset(datasetId);
        }
        // [END create_dataset]

        // [START create_table]
        public void CreateTable(string datasetId, string tableId, BigqueryClient client)
        {
            var dataset = client.GetDataset(datasetId);
            // Create schema for new table.
            var schema = new TableSchemaBuilder
            {
                { "title", BigqueryDbType.String },
                { "unique_words", BigqueryDbType.Integer }
            }.Build();
            // Create the table if it doesn't exist.
            BigqueryTable table = dataset.GetOrCreateTable(tableId, schema);
        }
        // [END create_table]

        public void DeleteDataset(string datasetId, BigqueryClient client)
        {
            client.DeleteDataset(datasetId);
        }

        // [START delete_table]
        public void DeleteTable(string datasetId, string tableId, BigqueryClient client)
        {
            // The Bigquery client method DeleteTable() is currently failing as noted in the following issue:
            // https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/443
            client.DeleteTable(_projectId, datasetId, tableId);
        }
        // [END delete_table]

        // [START import_file_from_gcs]
        public void ImportDataFromCloudStorage(string projectId, string datasetId,
            string tableId, BigqueryClient client, string fileName, string folder = null)
        {
            StorageClient gcsClient = StorageClient.Create();

            using (var stream = new MemoryStream())
            {
                // Set Cloud Storage Bucket name. This uses a bucket named the same as the project.
                string bucket = projectId;
                // If folder is passed in, add it to Cloud Storage File Path using "/" character
                string filePath = string.IsNullOrEmpty(folder) ? fileName : folder + "/" + fileName;
                // Download Google Cloud Storage object into stream
                gcsClient.DownloadObject(projectId, filePath, stream);

                // This example uploads data to an existing table. If the upload will create a new table
                // or if the schema in the JSON isn't identical to the schema in the table,
                // create a schema to pass into the call instead of passing in a null value.
                BigqueryJob job = client.UploadJson(datasetId, tableId, null, stream);
                // Use the job to find out when the data has finished being inserted into the table,
                // report errors etc.

                // Wait for the job to complete.
                job.PollUntilCompleted();
            }
        }
        // [END import_file_from_gcs]

        // [START sync_query]
        public BigqueryQueryJob SyncQuery(string projectId, string datasetId, string tableId,
            string query, double timeoutMs, BigqueryClient client)
        {
            var table = client.GetTable(projectId, datasetId, tableId);
            BigqueryJob job = client.CreateQueryJob(query,
                new CreateQueryJobOptions { UseQueryCache = false });
            // Get the query result, waiting for the timespan specified in milliseconds.
            BigqueryQueryJob result = client.GetQueryResults(job.Reference.JobId,
                new GetQueryResultsOptions { Timeout = TimeSpan.FromMilliseconds(timeoutMs) });
            return result;
        }
        // [END sync_query]

        // [START sync_query_legacy_sql]
        public BigqueryQueryJob LegacySqlSyncQuery(string projectId, string datasetId,
            string tableId, string query, double timeoutMs, BigqueryClient client)
        {
            var table = client.GetTable(projectId, datasetId, tableId);
            BigqueryJob job = client.CreateQueryJob(query,
                new CreateQueryJobOptions { UseLegacySql = true });
            // Get the query result, waiting for the timespan specified in milliseconds.
            BigqueryQueryJob result = client.GetQueryResults(job.Reference.JobId,
                new GetQueryResultsOptions { Timeout = TimeSpan.FromMilliseconds(timeoutMs) });
            return result;
        }
        // [END sync_query_legacy_sql]

        // [START async_query]
        public BigqueryQueryJob AsyncQuery(string projectId, string datasetId, string tableId,
            string query, BigqueryClient client)
        {
            var table = client.GetTable(projectId, datasetId, tableId);
            BigqueryJob job = client.CreateQueryJob(query,
                new CreateQueryJobOptions { UseQueryCache = false });

            // Wait for the job to complete.
            job.PollUntilCompleted();

            // Then we can fetch the results, either via the job or by accessing
            // the destination table.
            return client.GetQueryResults(job.Reference.JobId);
        }
        // [START async_query]


        // [START import_from_file]
        public void UploadJsonFromFile(string projectId, string datasetId, string tableId,
            string fileName, BigqueryClient client)
        {
            using (FileStream stream = File.Open(fileName, FileMode.Open))
            {
                // This example uploads data to an existing table. If the upload will create a new table
                // or if the schema in the JSON isn't identical to the schema in the table,
                // create a schema to pass into the call instead of passing in a null value.
                BigqueryJob job = client.UploadJson(datasetId, tableId, null, stream);
                // Use the job to find out when the data has finished being inserted into the table,
                // report errors etc.

                // Wait for the job to complete.
                job.PollUntilCompleted();
            }
        }
        // [END import_from_file]

        // [START stream_row]
        public void UploadJson(string datasetId, string tableId, BigqueryClient client)
        {
            // Note that there's a single line per JSON object. This is not a JSON array.
            IEnumerable<string> jsonRows = new string[]
            {
                "{ 'title': 'exampleJsonFromStream', 'unique_words': 1}",
                "{ 'title': 'moreExampleJsonFromStream', 'unique_words': 1}",
                //add more rows here...
            }.Select(row => row.Replace('\'', '"')); // Simple way of representing C# in JSON to avoid escaping " everywhere.

            // Normally we'd be uploading from a file or similar. Any readable stream can be used.
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", jsonRows)));

            // This example uploads data to an existing table. If the upload will create a new table
            // or if the schema in the JSON isn't identical to the schema in the table,
            // create a schema to pass into the call instead of passing in a null value.
            BigqueryJob job = client.UploadJson(datasetId, tableId, null, stream);
            // Use the job to find out when the data has finished being inserted into the table,
            // report errors etc.

            // Wait for the job to complete.
            job.PollUntilCompleted();
        }
        // [END stream_row]

        // [START export_to_cloud_storage]
        public void ExportJsonToGcs(
            string datasetId, string tableId, string bucketName, string fileName, BigqueryClient client)
        {
            StorageClient gcsClient = StorageClient.Create();
            string contentType = "application/json";
            // Get Table and append results into StringBuilder.
            var result = client.ListRows(datasetId, tableId);
            StringBuilder sb = new StringBuilder();
            foreach (var row in result)
            {
                sb.Append($"{{\"title\" : \"{row["title"]}\", \"unique_words\":\"{row["unique_words"]}\"}}{Environment.NewLine}");
            }
            // Save stream to Google Cloud Storage.
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
            {
                var obj = gcsClient.UploadObject(bucketName, fileName, contentType, stream);
            }
        }
        // [END export_to_cloud_storage]

        public void ExportCsvToGcs(
            string datasetId, string tableId, string bucketName, string fileName, BigqueryClient client)
        {
            StorageClient gcsClient = StorageClient.Create();
            string contentType = "text/csv";
            // Get Table and input results into StringBuilder.
            var result = client.ListRows(datasetId, tableId);
            StringBuilder sb = new StringBuilder();
            // Create header row.
            sb.Append($"title, unique_words,{Environment.NewLine}");
            foreach (var row in result)
            {
                sb.Append($"{row["title"]}, {row["unique_words"]},{Environment.NewLine}");
            }
            // Save stream to Google Cloud Storage.
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())))
            {
                var obj = gcsClient.UploadObject(bucketName, fileName, contentType, stream);
            }
        }

        public void PopulateTable(
            string query, string datasetId, string newTableId, BigqueryClient client)
        {
            var destination = client.GetTableReference(datasetId, newTableId);
            BigqueryJob job = client.CreateQueryJob(query,
                new CreateQueryJobOptions { DestinationTable = destination });
            // Wait for the job to complete.
            job.PollQueryUntilCompleted();
        }

        // [START copy_table]
        public void CopyTable(
            string datasetId, string tableIdToBeCopied, string newTableId, BigqueryClient client)
        {
            var table = client.GetTable(datasetId, tableIdToBeCopied);
            string query = $"SELECT * FROM {table}";
            var destination = client.GetTableReference(datasetId, newTableId);
            BigqueryJob job = client.CreateQueryJob(query,
                new CreateQueryJobOptions { DestinationTable = destination });
            // Wait for the job to complete.
            job.PollQueryUntilCompleted();
        }
        // [END copy_table]

        public string GetFileNameFromCloudStorage(string bucket, string fileName)
        {
            StorageClient gcsClient = StorageClient.Create();
            var file = gcsClient.GetObject(bucket, fileName);
            return file.Name;
        }

        public void DeleteFileFromGcs(string bucket, string fileName)
        {
            StorageClient gcsClient = StorageClient.Create();
            gcsClient.DeleteObject(bucket, fileName);
        }

        // [START list_datasets]
        public List<BigqueryDataset> ListDatasets(BigqueryClient client)
        {
            var datasets = client.ListDatasets().ToList();
            return datasets;
        }
        // [END list_datasets]

        // [START list_projects]
        public List<CloudProject> ListProjects(BigqueryClient client)
        {
            var projects = client.ListProjects().ToList();
            return projects;
        }
        // [END list_projects]

        // [START list_rows]
        public int ListRows(
            string projectId, string datasetId, string tableId, int numberOfRows, BigqueryClient client)
        {
            int recordCount = 0;
            var result = client.ListRows(projectId, datasetId, tableId, null,
                new ListRowsOptions { PageSize = numberOfRows });
            foreach (var row in result.Take(numberOfRows))
            {
                Console.WriteLine($"{row["word"]}: {row["corpus"]}");
                recordCount++;
            }
            return recordCount;
        }
        // [END list_rows]

        // [START browse_table]
        public int TableDataList(
            string datasetId, string tableId, int pageSize, BigqueryClient client)
        {
            int recordCount = 0;
            var result = client.ListRows(datasetId, tableId, null,
                new ListRowsOptions { PageSize = pageSize });
            // If there are more rows than were returned in the first page of results, 
            // iterating over the rows will lazily evaluate the results each time, 
            // making further requests as necessary.
            foreach (var row in result)
            {
                Console.WriteLine($"{row["title"]}: {row["unique_words"]}");
                recordCount++;
            }
            return recordCount;
        }
        // [END browse_table]

        private string GetConsoleAppOutput(string filePath)
        {
            string output;
            Process consoleApp = new Process();
            consoleApp.StartInfo.FileName = filePath;
            consoleApp.StartInfo.UseShellExecute = false;
            consoleApp.StartInfo.RedirectStandardOutput = true;
            consoleApp.Start();

            output = consoleApp.StandardOutput.ReadToEnd();

            consoleApp.WaitForExit();
            return output;
        }

        [Fact]
        public void TestQuickStartConsoleApp()
        {
            string output;
            string filePath = "..\\..\\..\\Quickstart\\bin\\Debug\\QuickStart.exe";
            string expectedOutputFirstWord = "Dataset";
            string expectedOutputLastWord = "created.";
            string sampleDatasetUsedInQuickStart = "my_new_dataset";
            output = GetConsoleAppOutput(filePath).Trim();
            var outputParts = output.Split(new[] { ' ' });
            Assert.Equal(outputParts.First(), expectedOutputFirstWord);
            Assert.Equal(outputParts.Last(), expectedOutputLastWord);
            //Delete QuickStart testing Dataset if it exists
            var dataset = _client.GetDataset(sampleDatasetUsedInQuickStart);
            if (dataset != null) { DeleteDataset(sampleDatasetUsedInQuickStart, _client); };
        }

        [Fact]
        public void TestSyncQuery()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            var table = _client.GetTable(projectId, datasetId, tableId);
            string query = $@"SELECT corpus AS title, COUNT(*) AS unique_words FROM {table}
                GROUP BY title ORDER BY unique_words DESC LIMIT 42";
            BigqueryQueryJob results = SyncQuery(projectId, datasetId, tableId, query, 10000, _client);
            Assert.True(results.GetRows().Count() > 0);
        }

        [Fact]
        public void TestLegacySqlSyncQuery()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            var table = _client.GetTable(projectId, datasetId, tableId);
            string query = $"SELECT TOP(corpus, 42) as title, COUNT(*) as unique_words FROM [{table.FullyQualifiedId}]";
            BigqueryQueryJob results = LegacySqlSyncQuery(
                projectId, datasetId, tableId, query, 10000, _client);
            Assert.True(results.GetRows().Count() > 0);
        }

        [Fact]
        public void TestAsyncQuery()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            var table = _client.GetTable(projectId, datasetId, tableId);
            string query = $@"SELECT corpus AS title, COUNT(*) AS unique_words FROM {table} 
                GROUP BY title ORDER BY unique_words DESC LIMIT 42";
            BigqueryQueryJob results = AsyncQuery(projectId, datasetId, tableId, query, _client);
            Assert.True(results.GetRows().Count() > 0);
        }

        [Fact]
        public void TestListDataset()
        {
            string datasetId = "sampleDatasetTestListDataset";
            CreateDataset(datasetId, _client);
            var datasets = ListDatasets(_client);
            Assert.True(datasets.Count() > 0);
            DeleteDataset(datasetId, _client);
        }

        [Fact]
        public void TestListProjects()
        {
            var projects = ListProjects(_client);
            Assert.True(projects.Count() > 0);
        }

        [Fact]
        public void TestBrowseRows()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            int numberOfRows = 20;
            int recordCount = ListRows(projectId, datasetId, tableId, numberOfRows, _client);
            Assert.True(recordCount == numberOfRows);
        }

        [Fact]
        public void TestCreateTable()
        {
            string datasetId = "datasetForTestCreateTable";
            string newTableId = "tableForTestCreateTable";
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            // Get created table.
            var newTable = _client.GetTable(datasetId, newTableId);
            // Confirm created table name equals expected name.
            Assert.Equal(newTableId, newTable.Reference.TableId);
            DeleteTable(datasetId, newTableId, _client);
            DeleteDataset(datasetId, _client);
        }

        [Fact]
        public void TestImportFromCloudStorage()
        {
            string datasetId = "datasetForTestImportFromCloudStorage";
            string newTableId = "tableForTestImportFromCloudStorage";
            string jsonGcsSampleFile = "sample.json";
            string gcsFolder = "test";
            string gcsUploadTestWord = "exampleJsonFromGCS";
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            // Import data.
            ImportDataFromCloudStorage(_projectId, datasetId, newTableId, _client,
                jsonGcsSampleFile, gcsFolder);
            // Run query to get table data.
            var newTable = _client.GetTable(datasetId, newTableId);
            string query = $"SELECT title, unique_words FROM {newTable}";
            BigqueryQueryJob results = AsyncQuery(_projectId, datasetId, newTableId,
                query, _client);
            // Get first row and confirm it contains the expected value.
            var row = results.GetRows().First();
            Assert.Equal(gcsUploadTestWord, row["title"]);
            DeleteTable(datasetId, newTableId, _client);
            DeleteDataset(datasetId, _client);
        }

        [Fact]
        public void TestListTables()
        {
            string datasetId = "datasetForTestListTables";
            string newTableId = "tableForTestListTables";
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            // [START list_tables]
            var tables = _client.ListTables(datasetId).ToList();
            // [END list_tables]
            Assert.False(tables.Count() == 0);
            DeleteTable(datasetId, newTableId, _client);
            DeleteDataset(datasetId, _client);
        }

        [Fact]
        public void TestImportDataFromFile()
        {
            string datasetId = "datasetForTestImportDataFromFile";
            string newTableId = "tableForTestImportDataFromFile";
            string uploadTestWord = "additionalExampleJsonFromFile";
            long uploadTestWordValue = 9814072356;
            string filePath = "..\\..\\..\\test\\data\\sample.json";
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            // Import data.
            UploadJsonFromFile(_projectId, datasetId, newTableId, filePath, _client);
            // Query table to get first row and confirm it contains the expected value
            var newTable = _client.GetTable(datasetId, newTableId);
            string query = $"SELECT title, unique_words FROM {newTable} WHERE title = '{uploadTestWord}'";
            BigqueryQueryJob results = AsyncQuery(_projectId, datasetId, newTableId, query, _client);
            var row = results.GetRows().Last();
            Assert.Equal(uploadTestWordValue, row["unique_words"]);
            DeleteTable(datasetId, newTableId, _client);
            DeleteDataset(datasetId, _client);
        }

        [Fact]
        public void TestImportDataFromStream()
        {
            string datasetId = "datasetForTestImportDataFromStream";
            string newTableId = "tableForTestImportDataFromStream";
            string gcsUploadTestWord = "exampleJsonFromStream";
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            // Import data.
            UploadJson(datasetId, newTableId, _client);
            // Query table to get first row and confirm it contains the expected value.
            var newTable = _client.GetTable(datasetId, newTableId);
            string query = $"SELECT title, unique_words FROM {newTable}";
            BigqueryQueryJob results = AsyncQuery(_projectId, datasetId, newTableId, query, _client);
            var row = results.GetRows().First();
            Assert.Equal(gcsUploadTestWord, row["title"]);
            DeleteTable(datasetId, newTableId, _client);
            DeleteDataset(datasetId, _client);
        }

        [Fact]
        public void TestExportJsonToCloudStorage()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            string newDatasetId = "datasetForTestExportJsonToCloudStorage";
            string newTableId = "tableForTestExportJsonToCloudStorage";
            string fileName = "uploaded.json";
            CreateDataset(newDatasetId, _client);
            CreateTable(newDatasetId, newTableId, _client);
            // Create Query.
            var table = _client.GetTable(projectId, datasetId, tableId);
            string query = $@"SELECT corpus AS title, COUNT(*) AS unique_words FROM {table} 
                GROUP BY title ORDER BY unique_words DESC LIMIT 42";
            // Populate Table.
            PopulateTable(query, newDatasetId, newTableId, _client);
            // Export Table to Cloud Storage.
            ExportJsonToGcs(newDatasetId, newTableId, _projectId, fileName, _client);
            // Get file details from Cloud Storage.
            // This sample code uses a bucket named the same as the projectId.
            string fileNameFromCloudStorage = GetFileNameFromCloudStorage(_projectId, fileName);
            // Assert that the filename from Cloud Storage equals expected filename.
            Assert.Equal(fileName, fileNameFromCloudStorage);
            // Delete file from Cloud Storage.
            DeleteFileFromGcs(_projectId, fileName);
            DeleteTable(newDatasetId, newTableId, _client);
            DeleteDataset(newDatasetId, _client);
        }

        [Fact]
        public void TestExportCsvToCloudStorage()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            string newDatasetId = "datasetForTestExportCsvToCloudStorage";
            string newTableId = "tableForTestExportCsvToCloudStorage";
            string fileName = "uploaded.csv";
            CreateDataset(newDatasetId, _client);
            CreateTable(newDatasetId, newTableId, _client);
            // Create Query
            var table = _client.GetTable(projectId, datasetId, tableId);
            string query = $@"SELECT corpus AS title, COUNT(*) AS unique_words FROM {table} 
                GROUP BY title ORDER BY unique_words DESC LIMIT 42";
            // Populate Table
            PopulateTable(query, newDatasetId, newTableId, _client);
            // Export Table to Cloud Storage.
            ExportCsvToGcs(newDatasetId, newTableId, _projectId, fileName, _client);
            // Get file details from Cloud Storage.
            // This sample code uses a bucket named the same as the projectId.
            string fileNameFromCloudStorage = GetFileNameFromCloudStorage(_projectId, fileName);
            // Assert that the filename from Cloud Storage equals expected filename.
            Assert.Equal(fileName, fileNameFromCloudStorage);
            // Delete file from Cloud Storage.
            DeleteFileFromGcs(_projectId, fileName);
            DeleteTable(newDatasetId, newTableId, _client);
            DeleteDataset(newDatasetId, _client);
        }

        [Fact]
        public void TestBrowseTable()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            string newDatasetId = "datasetForTestBrowseTable";
            string newTableId = "tableForTestBrowseTable";
            int pageSize = 5;
            int expectedNumberOfRecords = 42;
            CreateDataset(newDatasetId, _client);
            CreateTable(newDatasetId, newTableId, _client);
            // Create Query.
            var table = _client.GetTable(projectId, datasetId, tableId);
            string query = $@"SELECT corpus AS title, COUNT(*) AS unique_words FROM {table} 
                GROUP BY title ORDER BY unique_words DESC LIMIT 42";
            // Populate Table.
            PopulateTable(query, newDatasetId, newTableId, _client);
            // Get result from table to confirm expected number of records.
            int recordCount = TableDataList(newDatasetId, newTableId, pageSize, _client);
            Assert.True(recordCount == expectedNumberOfRecords);
            DeleteTable(newDatasetId, newTableId, _client);
            DeleteDataset(newDatasetId, _client);
        }

        [Fact]
        public void TestCopyTable()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            string newDatasetId = "datasetForTestCopyTable";
            string sourceTableId = "tableForTestCopyTable";
            string copiedTableId = "copiedTableForTestCopyTable";
            CreateDataset(newDatasetId, _client);
            CreateTable(newDatasetId, sourceTableId, _client);
            // Create Query.
            var table = _client.GetTable(projectId, datasetId, tableId);
            string query = $@"SELECT corpus AS title, COUNT(*) AS unique_words FROM {table} 
                GROUP BY title ORDER BY unique_words DESC LIMIT 42";
            // Populate Table.
            PopulateTable(query, newDatasetId, sourceTableId, _client);
            // Copy Table.
            CopyTable(newDatasetId, sourceTableId, copiedTableId, _client);
            // Get rows from new copied Table to confirm copy success.
            var copyTable = _client.GetTable(newDatasetId, copiedTableId);
            var result = copyTable.ListRows();
            Assert.False(result.Count() == 0);
            // Delete copied table.
            DeleteTable(newDatasetId, copiedTableId, _client);
            // Delete source table.
            DeleteTable(newDatasetId, sourceTableId, _client);
            DeleteDataset(newDatasetId, _client);
        }
    }
}
