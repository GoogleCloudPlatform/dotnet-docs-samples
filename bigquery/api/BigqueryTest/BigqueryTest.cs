/*
 * Copyright (c) 2017 Google Inc.
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

using System;
// [START bigquery_client_default_credentials]
using Google.Cloud.BigQuery.V2;
// [END bigquery_client_default_credentials]
using Google.Cloud.Storage.V1;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.IO;
using System.Diagnostics;
using Google.Apis.Bigquery.v2.Data;
using Google.Api.Gax;

namespace GoogleCloudSamples
{
    public class BigQueryTest : IDisposable
    {
        private readonly string _projectId;
        private readonly BigQueryClient _client;
        // Create list of tuples to collect tableName and datasetName for test tables to be deleted.
        private readonly List<Tuple<string, string>> _tablesToDelete = new List<Tuple<string, string>>();
        private readonly List<string> _datasetsToDelete = new List<string>();
        private readonly List<string> _filesToDelete = new List<string>();
        private static readonly Random s_random = new Random();
        // Generate random suffix for creating unique resource names.
        private static string RandomSuffix()
        {
            return s_random.Next(1000000).ToString();
        }

        public void Dispose()
        {
            try
            {
                // Delete all tables created from running the tests.
                foreach (Tuple<string, string> resource in _tablesToDelete)
                {
                    DeleteTable(resource.Item1, resource.Item2, _client);
                }
            }
            catch (Google.GoogleApiException ex) when
                (ex != null
                    && ex.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
            { }
            try
            {
                // Delete all datasets created from running the tests.
                foreach (string dataset in _datasetsToDelete)
                {
                    DeleteDataset(dataset, _client);
                }
            }
            catch (Google.GoogleApiException ex) when
                (ex != null
                    && ex.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
            { }
            try
            {
                // Delete all cloud storage files created from running the tests.
                foreach (string fileName in _filesToDelete)
                {
                    DeleteFileFromGcs(_projectId, fileName);
                }
            }
            catch (Google.GoogleApiException ex) when
                (ex != null
                    && ex.HttpStatusCode == System.Net.HttpStatusCode.BadRequest)
            { }
        }

        private readonly RetryRobot _retryDeleteBusy = new RetryRobot
        {
            ShouldRetry = (Exception e) =>
            {
                // Retry when we see:
                // Dataset yada:yada is still in use [400]
                var apiException = e as Google.GoogleApiException;
                return apiException != null &&
                    apiException.HttpStatusCode == System.Net.HttpStatusCode.BadRequest;
            }
        };

        public BigQueryTest()
        {
            // [START bigquery_client_default_credentials]
            // By default, the Google.Bigquery.V2 library client will authenticate
            // using the service account file (created in the Google Developers
            // Console) specified by the GOOGLE_APPLICATION_CREDENTIALS
            // environment variable. If you are running on
            // a Google Compute Engine VM, authentication is completely
            // automatic.
            _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _client = BigQueryClient.Create(_projectId);
            // [END bigquery_client_default_credentials]
        }

        // [START bigquery_create_dataset]
        public void CreateDataset(string datasetId, BigQueryClient client)
        {
            var dataset = client.GetOrCreateDataset(datasetId);
        }
        // [END bigquery_create_dataset]

        // [START bigquery_create_table]
        public void CreateTable(string datasetId, string tableId, BigQueryClient client)
        {
            var dataset = client.GetDataset(datasetId);
            // Create schema for new table.
            var schema = new TableSchemaBuilder
            {
                { "title", BigQueryDbType.String },
                { "unique_words", BigQueryDbType.Int64 }
            }.Build();
            // Create the table if it doesn't exist.
            BigQueryTable table = dataset.GetOrCreateTable(tableId, schema);
        }
        // [END bigquery_create_table]

        public void DeleteDataset(string datasetId, BigQueryClient client)
        {
            _retryDeleteBusy.Eventually(() => client.DeleteDataset(datasetId));
        }

        // [START bigquery_delete_table]
        public void DeleteTable(string datasetId, string tableId, BigQueryClient client)
        {
            client.DeleteTable(_projectId, datasetId, tableId);
        }
        // [END bigquery_delete_table]

        // [START bigquery_query_legacy]
        public BigQueryResults LegacySqlAsyncQuery(string projectId, string datasetId,
            string tableId, string query, BigQueryClient client)
        {
            var table = client.GetTable(projectId, datasetId, tableId);
            BigQueryJob job = client.CreateQueryJob(query,
                parameters: null,
                options: new QueryOptions { UseLegacySql = true });

            // Wait for the job to complete.
            job.PollUntilCompleted();

            // Then we can fetch the results, either via the job or by accessing
            // the destination table.
            return client.GetQueryResults(job.Reference.JobId);
        }
        // [END bigquery_query_legacy]

        // [START bigquery_query]
        public BigQueryResults AsyncQuery(string projectId, string datasetId, string tableId,
            string query, BigQueryClient client)
        {
            var table = client.GetTable(projectId, datasetId, tableId);
            BigQueryJob job = client.CreateQueryJob(query,
                parameters: null,
                options: new QueryOptions { UseQueryCache = false });

            // Wait for the job to complete.
            job.PollUntilCompleted();

            // Then we can fetch the results, either via the job or by accessing
            // the destination table.
            return client.GetQueryResults(job.Reference.JobId);
        }
        // [END bigquery_query]


        // [START bigquery_load_from_file]
        public void UploadJsonFromFile(string projectId, string datasetId, string tableId,
            string fileName, BigQueryClient client)
        {
            using (FileStream stream = File.Open(fileName, FileMode.Open))
            {
                // This example uploads data to an existing table. If the upload will create a new table
                // or if the schema in the JSON isn't identical to the schema in the table,
                // create a schema to pass into the call instead of passing in a null value.
                BigQueryJob job = client.UploadJson(datasetId, tableId, null, stream);
                // Use the job to find out when the data has finished being inserted into the table,
                // report errors etc.

                // Wait for the job to complete.
                job.PollUntilCompleted();
            }
        }
        // [END bigquery_load_from_file]

        // [START bigquery_table_insert_rows]
        public void UploadJsonStreaming(string datasetId, string tableId,
            BigQueryClient client)
        {
            // The insert ID is optional, but can avoid duplicate data
            // when retrying inserts.
            BigQueryInsertRow row1 = new BigQueryInsertRow("row1")
            {
                { "title", "exampleJsonFromStream" },
                { "unique_words", 1 }
            };
            BigQueryInsertRow row2 = new BigQueryInsertRow("row2")
            {
                { "title", "moreExampleJsonFromStream" },
                { "unique_words", 1 }
            };
            client.InsertRows(datasetId, tableId, row1, row2);
        }
        // [END bigquery_table_insert_rows]

        // [START bigquery_extract_table]
        public void ExportJsonToGcs(
            string datasetId, string tableId, string bucketName, string fileName,
            BigQueryClient client)
        {
            StorageClient gcsClient = StorageClient.Create();
            string contentType = "application/json";
            // Get Table and append results into StringBuilder.
            PagedEnumerable<TableDataList, BigQueryRow> result = client.ListRows(datasetId, tableId);
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
        // [END bigquery_extract_table]

        public void ExportCsvToGcs(
            string datasetId, string tableId, string bucketName, string fileName, BigQueryClient client)
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
            string query, string datasetId, string newTableId, BigQueryClient client)
        {
            var destination = client.GetTableReference(datasetId, newTableId);
            BigQueryJob job = client.CreateQueryJob(query,
                parameters: null,
                options: new QueryOptions { DestinationTable = destination });
            // Wait for the job to complete.
            job.GetQueryResults();
        }

        // [START bigquery_copy_table]
        public void CopyTable(
            string datasetId, string tableIdToBeCopied, string newTableId, BigQueryClient client)
        {
            var table = client.GetTable(datasetId, tableIdToBeCopied);
            string query = $"SELECT * FROM {table}";
            var destination = client.GetTableReference(datasetId, newTableId);
            BigQueryJob job = client.CreateQueryJob(query,
                parameters: null,
                options: new QueryOptions { DestinationTable = destination });
            // Wait for the job to complete.
            job.GetQueryResults();
        }
        // [END bigquery_copy_table]

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

        // [START bigquery_list_datasets]
        public List<BigQueryDataset> ListDatasets(BigQueryClient client)
        {
            var datasets = client.ListDatasets().ToList();
            return datasets;
        }
        // [END bigquery_list_datasets]

        // [START bigquery_list_tables]
        public List<BigQueryTable> ListTables(BigQueryClient client, string datasetId)
        {
            var tables = client.ListTables(datasetId).ToList();
            return tables;
        }
        // [END bigquery_list_tables]

        // [START bigquery_browse_table]
        public int TableDataList(
            string datasetId, string tableId, int pageSize, BigQueryClient client)
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
        // [END bigquery_browse_table]

        [Fact]
        public void TestLegacySqlAsyncQuery()
        {
            string projectId = "bigquery-public-data";
            string datasetId = "samples";
            string tableId = "shakespeare";
            var table = _client.GetTable(projectId, datasetId, tableId);
            string query = $"SELECT TOP(corpus, 42) as title, COUNT(*) as unique_words FROM [{table.FullyQualifiedId}]";
            BigQueryResults results = LegacySqlAsyncQuery(
                projectId, datasetId, tableId, query, _client);
            Assert.True(results.Count() > 0);
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
            BigQueryResults results = AsyncQuery(projectId, datasetId, tableId, query, _client);
            Assert.True(results.Count() > 0);
        }

        [Fact]
        public void TestListDataset()
        {
            string datasetId = "sampleDatasetTestListDataset";
            _datasetsToDelete.Add(datasetId);
            CreateDataset(datasetId, _client);
            var datasets = ListDatasets(_client);
            Assert.True(datasets.Count() > 0);
        }

        [Fact]
        public void TestCreateTable()
        {
            string datasetId = "datasetForTestCreateTable";
            string newTableId = "tableForTestCreateTable";
            _tablesToDelete.Add(new Tuple<string, string>(datasetId, newTableId));
            _datasetsToDelete.Add(datasetId);
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            // Get created table.
            var newTable = _client.GetTable(datasetId, newTableId);
            // Confirm created table name equals expected name.
            Assert.Equal(newTableId, newTable.Reference.TableId);
        }

        [Fact]
        public void TestImportFromCloudStorageCSV()
        {
            var datasetId = $"datasetForLoadCSV{DateTime.Now.Ticks}";
            var newTableID = $"tableForTestImportDataFromCSV{RandomSuffix()}";
            _tablesToDelete.Add(new Tuple<string, string>(datasetId, newTableID));
            _datasetsToDelete.Add(datasetId);
            // Test parameters.
            string expectedFirstRowName = "Alabama";

            // Import data.
            GoogleCloudSamples.BiqQuerySnippets.LoadTableFromCSV(datasetId, newTableID, _client);

            // Run query to get table data.
            var newTable = _client.GetTable(datasetId, newTableID);
            string query = $"SELECT name, post_abbr FROM {newTable}" +
                $"ORDER BY name, post_abbr";

            BigQueryResults results = AsyncQuery(_projectId, datasetId, newTableID,
                query, _client);
            var row = results.First();

            // Check results.
            Assert.Equal(expectedFirstRowName, row["name"]);
            Assert.True(results.Count() == 50);
        }

        [Fact]
        public void TestImportFromCloudStorageOrc()
        {
            var datasetId = $"datasetForLoadOrc{DateTime.Now.Ticks}";
            var newTableID = $"tableForTestImportDataFromOrc{RandomSuffix()}";
            _tablesToDelete.Add(new Tuple<string, string>(datasetId, newTableID));
            _datasetsToDelete.Add(datasetId);
            // Test parameters.
            string expectedFirstRowName = "Alabama";

            // Import data.
            GoogleCloudSamples.BiqQuerySnippets.LoadTableFromOrc(datasetId, newTableID, _client);

            // Run query to get table data.
            var newTable = _client.GetTable(datasetId, newTableID);
            string query = $"SELECT name, post_abbr FROM {newTable}" +
                $"ORDER BY name, post_abbr";

            BigQueryResults results = AsyncQuery(_projectId, datasetId, newTableID,
                query, _client);
            var row = results.First();

            // Check results.
            Assert.Equal(expectedFirstRowName, row["name"]);
            Assert.True(results.Count() == 50);
        }

        [Fact]
        public void TestListTables()
        {
            string datasetId = "datasetForTestListTables";
            string newTableId = "tableForTestListTables";
            _tablesToDelete.Add(new Tuple<string, string>(datasetId, newTableId));
            _datasetsToDelete.Add(datasetId);
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            var tables = ListTables(_client, datasetId);
            Assert.True(tables.Count() > 0);
        }

        [Fact]
        public void TestImportDataFromFile()
        {
            string datasetId = "datasetForTestImportDataFromFile";
            string newTableId = "tableForTestImportDataFromFile";
            _tablesToDelete.Add(new Tuple<string, string>(datasetId, newTableId));
            _datasetsToDelete.Add(datasetId);
            string uploadTestWord = "additionalExampleJsonFromFile";
            long uploadTestWordValue = 9814072356;
            string filePath = Path.Combine("data", "sample.json");
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            // Import data.
            UploadJsonFromFile(_projectId, datasetId, newTableId, filePath, _client);
            // Query table to get first row and confirm it contains the expected value
            var newTable = _client.GetTable(datasetId, newTableId);
            string query = $"SELECT title, unique_words FROM {newTable} WHERE title = '{uploadTestWord}'";
            BigQueryResults results = AsyncQuery(_projectId, datasetId, newTableId, query, _client);
            var row = results.Last();
            Assert.Equal(uploadTestWordValue, row["unique_words"]);
        }

        [Fact]
        public void TestImportDataFromJSON()
        {
            var datasetId = $"datasetForLoadJson{DateTime.Now.Ticks}";
            var newTableID = $"tableForTestImportDataFromJSON{RandomSuffix()}";
            _tablesToDelete.Add(new Tuple<string, string>(datasetId, newTableID));
            _datasetsToDelete.Add(datasetId);

            // JSON file below has 50 items in it.
            string expectedFirstRowName = "Alabama";

            GoogleCloudSamples.BiqQuerySnippets.LoadTableFromJSON(_client, datasetId, newTableID);

            // Run query to get table data.
            var newTable = _client.GetTable(datasetId, newTableID);
            string query = $"SELECT name, post_abbr FROM {newTable}" +
                $"ORDER BY name, post_abbr";

            BigQueryResults results = AsyncQuery(_projectId, datasetId, newTableID,
                query, _client);
            var row = results.First();

            // Check results.
            Assert.Equal(expectedFirstRowName, row["name"]);
            Assert.True(results.Count() == 50);
        }

        [Fact]
        public void TestImportDataFromStream()
        {
            string datasetId = "datasetForTestImportDataFromStream";
            string newTableId = "tableForTestImportDataFromStream" + RandomSuffix();
            string gcsUploadTestWord = "exampleJsonFromStream";
            string valueToTest = "";
            _tablesToDelete.Add(new Tuple<string, string>(datasetId, newTableId));
            _datasetsToDelete.Add(datasetId);
            CreateDataset(datasetId, _client);
            CreateTable(datasetId, newTableId, _client);
            // Import data.
            UploadJsonStreaming(datasetId, newTableId, _client);
            // Query table to get first row and confirm it contains the expected value.
            var newTable = _client.GetTable(datasetId, newTableId);
            string query = $"SELECT title, unique_words FROM {newTable} ORDER BY title";
            try
            {
                var retryRobot = new RetryRobot();
                retryRobot.ShouldRetry = (ex) => ex is InvalidOperationException;
                retryRobot.FirstRetryDelayMs = 100;
                retryRobot.Eventually(() =>
                    {
                        BigQueryResults results = AsyncQuery(_projectId, datasetId, newTableId, query, _client);
                        var row = results.First();
                        valueToTest = row["title"].ToString();
                    });
            }
            catch (Exception)
            {
                // All of the retries failed.
            }

            Assert.Equal(gcsUploadTestWord, valueToTest);
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
            _tablesToDelete.Add(new Tuple<string, string>(newDatasetId, newTableId));
            _datasetsToDelete.Add(newDatasetId);
            _filesToDelete.Add(fileName);
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
            _tablesToDelete.Add(new Tuple<string, string>(newDatasetId, newTableId));
            _datasetsToDelete.Add(newDatasetId);
            _filesToDelete.Add(fileName);
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
            _tablesToDelete.Add(new Tuple<string, string>(newDatasetId, newTableId));
            _datasetsToDelete.Add(newDatasetId);
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
            _tablesToDelete.Add(new Tuple<string, string>(newDatasetId, copiedTableId));
            _tablesToDelete.Add(new Tuple<string, string>(newDatasetId, sourceTableId));
            _datasetsToDelete.Add(newDatasetId);
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
        }

        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = Program.Main,
            Command = "dotnet run"
        };

        [Fact]
        public void TestRunQuickStart()
        {
            string expectedOutputFirstWord = "Dataset";
            string expectedOutputLastWord = "created.";
            string sampleDatasetUsedInQuickStart = "my_new_dataset";
            _datasetsToDelete.Add(sampleDatasetUsedInQuickStart);
            var output = _quickStart.Run();
            Assert.Equal(0, output.ExitCode);
            var outputParts = output.Stdout.Split(new[] { ' ' });
            Assert.Equal(expectedOutputFirstWord, outputParts.First().Trim());
            Assert.Equal(expectedOutputLastWord, outputParts.Last().Trim());
        }
    }
}
