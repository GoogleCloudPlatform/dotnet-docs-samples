// Copyright(c) 2017 Google Inc.
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

using Google.Cloud.Spanner.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Sdk;
using Grpc.Core;

namespace GoogleCloudSamples.Spanner
{
    public class QuickStartTests
    {
        [Fact]
        public void TestQuickStart()
        {
            CommandLineRunner runner = new CommandLineRunner()
            {
                VoidMain = QuickStart.Main,
                Command = "QuickStart"
            };
            var result = runner.Run();
            Assert.Equal(0, result.ExitCode);
        }
    }

    public class SpannerFixture : IDisposable
    {
        public void Dispose()
        {
            try
            {
                // Delete database created from running the tests.
                CommandLineRunner runner = new CommandLineRunner()
                {
                    Main = Program.Main,
                    Command = "Spanner"
                };
                runner.Run("deleteDatabase",
                    ProjectId, InstanceId, DatabaseId);
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound) { }
        }

        public string ProjectId { get; private set; } =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // Allow environment variables to override the default instance and database names.
        public string InstanceId { get; private set; } =
            Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
        private static readonly string s_randomDatabaseName = "my-db-"
            + TestUtil.RandomName();
        public string DatabaseId =
            Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? s_randomDatabaseName;
        public bool s_initializedDatabase { get; set; } = false;
    }

    public class SpannerTests : IClassFixture<SpannerFixture>
    {
        readonly SpannerFixture _fixture;
        readonly CommandLineRunner _spannerCmd = new CommandLineRunner()
        {
            Main = Program.Main,
            Command = "Spanner"
        };

        public SpannerTests(SpannerFixture fixture)
        {
            _fixture = fixture;
            lock (this)
            {
                if (!_fixture.s_initializedDatabase)
                {
                    _fixture.s_initializedDatabase = true;
                    InitializeDatabase();
                }
            }
        }

        void InitializeDatabase()
        {
            // If the database has not been initialized, retry.
            _spannerCmd.Run("createSampleDatabase",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            _spannerCmd.Run("insertSampleData",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            _spannerCmd.Run("insertStructSampleData",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            _spannerCmd.Run("addColumn",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
        }

        async Task RefillMarketingBudgetsAsync(int firstAlbumBudget,
            int secondAlbumBudget)
        {
            string connectionString =
                $"Data Source=projects/{_fixture.ProjectId}/instances/{_fixture.InstanceId}"
                + $"/databases/{_fixture.DatabaseId}";
            // Create connection to Cloud Spanner.
            using (var connection =
                new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();
                var cmd = connection.CreateUpdateCommand("Albums",
                new SpannerParameterCollection
                {
                    {"SingerId", SpannerDbType.Int64},
                    {"AlbumId", SpannerDbType.Int64},
                    {"MarketingBudget", SpannerDbType.Int64},
                });
                for (int i = 1; i <= 2; ++i)
                {
                    cmd.Parameters["SingerId"].Value = i;
                    cmd.Parameters["AlbumId"].Value = i;
                    cmd.Parameters["MarketingBudget"].Value = i == 1 ?
                        firstAlbumBudget : secondAlbumBudget;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        [Fact]
        void TestQuery()
        {
            QuerySampleData();
        }

        [Fact]
        void TestQueryTransaction()
        {
            ConsoleOutput output = _spannerCmd.Run("queryDataWithTransaction",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestQueryTransactionCore()
        {
            ConsoleOutput output = _spannerCmd.Run("queryDataWithTransaction",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId, "netcore");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestReadWriteTransaction()
        {
            RefillMarketingBudgetsAsync(300000, 300000).Wait();
            ConsoleOutput output = _spannerCmd.Run("readWriteWithTransaction",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Transaction complete.", output.Stdout);
        }

        [Fact]
        void TestReadWriteUnderFundedTransaction()
        {
            RefillMarketingBudgetsAsync(300000, 299999).Wait();
            try
            {
                ConsoleOutput output = _spannerCmd.Run("readWriteWithTransaction",
                    _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            }
            catch (Exception e)
            {
                Assert.Contains("less than the minimum required amount",
                e.ToString());
            }
        }

        [Fact]
        void TestReadStaleData()
        {
            Thread.Sleep(TimeSpan.FromSeconds(16));
            ConsoleOutput output = _spannerCmd.Run("readStaleData",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Go, Go, Go", output.Stdout);
        }

        [Fact]
        void TestReadWriteTransactionCore()
        {
            RefillMarketingBudgetsAsync(300000, 300000).Wait();
            ConsoleOutput output = _spannerCmd.Run("readWriteWithTransaction",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId, "netcore");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Transaction complete.", output.Stdout);
        }

        [Fact]
        void TestReadWriteUnderFundedTransactionCore()
        {
            RefillMarketingBudgetsAsync(300000, 299999).Wait();
            try
            {
                ConsoleOutput output = _spannerCmd.Run("readWriteWithTransaction",
                    _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId, "netcore");
            }
            catch (Exception e)
            {
                Assert.Contains("less than the minimum required amount",
                e.ToString());
            }
        }

        [Fact]
        void TestBatchWriteAndRead()
        {
            // Batch insert records.
            ConsoleOutput insertOutput = _spannerCmd.Run("batchInsertRecords",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, insertOutput.ExitCode);
            // Batch read records.
            ConsoleOutput readOutput = _spannerCmd.Run("batchReadRecords",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm the output has valid numbers for row & partition count.
            // Match "[text] [comma separated number] [text] [number] [text]"
            var output_regex = new Regex(
                @"Total rows read: \d+(?:,\d{3})* with \d+ partition");
            var match = output_regex.Match(readOutput.Stdout);
            Assert.True(match.Success);
        }

        [Fact]
        void TestCommitTimestamp()
        {
            // Add a commit timestamp column to an existing table.
            ConsoleOutput insertOutput = _spannerCmd.Run("addCommitTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, insertOutput.ExitCode);
            // Update records with added commit timestamp column.
            ConsoleOutput updateOutput = _spannerCmd.Run("updateDataWithTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, updateOutput.ExitCode);
            // Query updated the records from the updated table.
            ConsoleOutput readOutput = _spannerCmd.Run("queryDataWithTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm timestamp column is included in output.
            Assert.Contains("LastUpdateTime :", readOutput.Stdout);
            // Create a new table that includes a commit timestamp column.
            ConsoleOutput createOutput = _spannerCmd.Run("createTableWithTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, createOutput.ExitCode);
            // Write data to the new table.
            ConsoleOutput writeOutput = _spannerCmd.Run("writeDataWithTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, writeOutput.ExitCode);
            // Query records from the new table.
            ConsoleOutput readNewTableOutput = _spannerCmd.Run("queryNewTableWithTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readNewTableOutput.ExitCode);
            // Confirm output includes valid timestamps.
            string columnText = "LastUpdateTime : ";
            string[] result = readNewTableOutput.Stdout.Split(
                new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string valueToTest = result[0].Substring(result[0].IndexOf(columnText) + columnText.Length);
            DateTime value;
            Assert.True(DateTime.TryParse(valueToTest, out value));
        }

        [Fact]
        void TestStructParameters()
        {
            // Query records using a Spanner struct.
            ConsoleOutput readOutput = _spannerCmd.Run("queryDataWithStruct",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("6", readOutput.Stdout);
            // Query records using an array of Spanner structs.
            readOutput = _spannerCmd.Run("queryDataWithArrayOfStruct",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("6", readOutput.Stdout);
            Assert.Contains("7", readOutput.Stdout);
            Assert.Contains("8", readOutput.Stdout);
            // Query records using a field value contained in a Spanner struct.
            readOutput = _spannerCmd.Run("queryDataWithStructField",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("6", readOutput.Stdout);
            // Query records using a field value contained in a nested Spanner struct.
            readOutput = _spannerCmd.Run("queryDataWithNestedStructField",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("6", readOutput.Stdout);
            Assert.Contains("Imagination", readOutput.Stdout);
            Assert.Contains("9", readOutput.Stdout);
            Assert.Contains("Imagination", readOutput.Stdout);
        }

        [Fact(Skip = "Triggers infinite loop described here: https://github.com/commandlineparser/commandline/commit/95ded2dbcc5285302723e68221cd30a72444ba84")]
        void TestSpannerNoArgsSucceeds()
        {
            ConsoleOutput output = _spannerCmd.Run();
            Assert.NotEqual(0, output.ExitCode);
        }

        [Fact]
        void TestCreateDatabase()
        {
            try
            {
                // Attempt to create another database with same name. Should fail.
                ConsoleOutput created_again = _spannerCmd.Run("createSampleDatabase",
                    _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            }
            catch (AggregateException e)
            {
                bool rethrow = true;
                foreach (var innerException in e.InnerExceptions)
                {
                    SpannerException spannerException = innerException as SpannerException;
                    if (spannerException != null && spannerException.Message.ToLower().Contains("duplicate"))
                    {
                        Console.WriteLine($"Database {_fixture.DatabaseId} already exists.");
                        rethrow = false;
                        break;
                    }
                }
                if (rethrow)
                {
                    throw;
                }
            }
            // List tables to confirm database tables exist.
            ConsoleOutput output = _spannerCmd.Run("listDatabaseTables",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Albums", output.Stdout);
            Assert.Contains("Singers", output.Stdout);
        }

        /// <summary>
        /// Run a couple queries and verify the database contains the
        /// data inserted by insertSampleData.
        /// </summary>
        /// <exception cref="XunitException">when an assertion fails.</exception>
        void QuerySampleData()
        {
            ConsoleOutput output = _spannerCmd.Run("querySampleData",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }
    }
}
