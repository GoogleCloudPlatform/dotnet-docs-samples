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

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Cloud.Spanner.Data;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GoogleCloudSamples.Spanner
{
    public class QuickStartFixture : IDisposable
    {
        public string ProjectId { get; private set; } =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        public string InstanceId { get; private set; } =
            Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
        public string DatabaseId { get; private set; } = "my-database";

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
    }
    public class QuickStartTests : IClassFixture<QuickStartFixture>
    {
        readonly QuickStartFixture _fixture;
        readonly CommandLineRunner _spannerCmd = new CommandLineRunner()
        {
            Main = Program.Main,
            Command = "Spanner"
        };

        public QuickStartTests(QuickStartFixture fixture)
        {
            _fixture = fixture;
            _spannerCmd.Run("createDatabase",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1062")]
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
                runner.Run("deleteDatabase",
                    ProjectId, InstanceId, RestoredDatabaseId);
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound) { }
        }

        public string ProjectId { get; private set; } =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // Allow environment variables to override the default instance and database names.
        public string InstanceId { get; private set; } =
            Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
        private static readonly string s_randomDatabaseName = "my-db-" + DateTime.UtcNow.ToTimestamp().Seconds;
        private static readonly string s_randomToBeCancelledBackupName = "my-backup-" + DateTime.UtcNow.ToTimestamp().Seconds;
        private static readonly string s_randomRestoredDatabaseName = "my-db-" + DateTime.UtcNow.ToTimestamp().Seconds;
        public string DatabaseId = Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? s_randomDatabaseName;
        public string BackupDatabaseId = "my-test-database";
        public string BackupId = "my-test-database-backup";
        public string ToBeCancelledBackupId = s_randomToBeCancelledBackupName;
        public string RestoredDatabaseId = s_randomRestoredDatabaseName;
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
                    DelelteBackups();
                    DelelteDatabases();
                    InitializeInstance();
                    InitializeDatabase();
                    InitializeBackup();
                }
            }
        }

        void DelelteBackups()
        {
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            //delete backup contains "a"
            var dataTime = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
            var listBackupRequest = new ListBackupsRequest
            {
                Parent = InstanceName.Format(_fixture.ProjectId, _fixture.InstanceId),
                Filter = $"create_time < {dataTime} AND name:Howl"
            };

            var backups = databaseAdminClient.ListBackups(listBackupRequest).ToList();
            Console.Out.WriteLineAsync($"{backups.Count} old backups found.");
            foreach (var backup in backups)
            {
                var deleteBackupRequest = new DeleteBackupRequest()
                {
                    Name = backup.Name
                };
                databaseAdminClient.DeleteBackup(deleteBackupRequest);
            }
        }

        void DelelteDatabases()
        {
            string adminConnectionString = $"Data Source=projects/{_fixture.ProjectId}/"
              + $"instances/{_fixture.InstanceId}";

            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();
            InstanceName instanceName = InstanceName.FromProjectInstance(_fixture.ProjectId, _fixture.InstanceId);
            var databases = databaseAdminClient.ListDatabases(instanceName).ToList();
            Console.Out.WriteLineAsync($"{databases.Count} old databases found.");
            using (var connection = new SpannerConnection(adminConnectionString))
                foreach (var database in databases)
                {
                    using (var cmd = connection.CreateDdlCommand($@"DROP DATABASE {database.DatabaseName.DatabaseId}"))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
        }

        void InitializeInstance()
        {
            InstanceAdminClient instanceAdminClient = InstanceAdminClient.Create();
            try
            {
                string name = $"projects/{_fixture.ProjectId}/instances/{_fixture.InstanceId}";
                Instance response = instanceAdminClient.GetInstance(name);
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
            {
                CreateInstance.SpannerCreateInstance(_fixture.ProjectId, _fixture.InstanceId);
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
            _spannerCmd.Run("addCommitTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            _spannerCmd.Run("addIndex",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            // Create a new table that includes supported datatypes.
            _spannerCmd.Run("createTableWithDatatypes",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            // Write data to the new table.
            _spannerCmd.Run("writeDatatypesData",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
        }

        void InitializeBackup()
        {
            // Sample database for backup and restore tests.
            try
            {
                _spannerCmd.Run("createSampleDatabase",
                    _fixture.ProjectId, _fixture.InstanceId, _fixture.BackupDatabaseId);
                _spannerCmd.Run("insertSampleData",
                    _fixture.ProjectId, _fixture.InstanceId, _fixture.BackupDatabaseId);
            }
            catch (Exception e)
            {
                // We intentionally keep an existing database around to reduce the
                // the likelihood of test timetouts when creating a backup so
                // it's ok to get an AlreadyExists error.
                if (e.ToString().Contains("Database already exists"))
                {
                    Console.WriteLine($"Database {_fixture.BackupDatabaseId} already exists.");
                }
                else
                {
                    throw;
                }
            }
            try
            {
                _spannerCmd.Run("createBackup",
                    _fixture.ProjectId, _fixture.InstanceId, _fixture.BackupDatabaseId,
                    _fixture.BackupId);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.AlreadyExists)
            {
                // We intentionally keep an existing backup around to reduce the
                // the likelihood of test timetouts when creating a backup so
                // it's ok to get an AlreadyExists error.
                if (e.ToString().Contains("Backup already exists"))
                {
                    Console.WriteLine($"Backup {_fixture.BackupId} already exists.");
                }
                else
                {
                    throw;
                }
            }
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
        void TestQueryDataWithIndex()
        {
            RefillMarketingBudgetsAsync(100000, 500000).Wait();
            ConsoleOutput output = _spannerCmd.Run("queryDataWithIndex",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("AlbumId : 2 AlbumTitle : Go, Go, Go MarketingBudget :", output.Stdout);
            Assert.Contains("AlbumId : 2 AlbumTitle : Forever Hold your Peace MarketingBudget : 500000", output.Stdout);
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
        void TestCommitTimestamp()
        {
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

        [Fact]
        void TestDeleteSampleData()
        {
            // The deleteSampleData command creates the necessary tables, populates it with data
            // and drops the tables afterwards.
            ConsoleOutput output = _spannerCmd.Run("deleteSampleData",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Contains("Deleted individual rows in UpcomingAlbums.", output.Stdout);
            Assert.Contains($"2 row(s) deleted from UpcomingSingers.", output.Stdout);
            Assert.Contains($"3 row(s) deleted from UpcomingSingers.", output.Stdout);
        }

        [Fact]
        void TestDml()
        {
            RefillMarketingBudgetsAsync(300000, 300000).Wait();
            // Insert record using a DML Statement.
            ConsoleOutput readOutput = _spannerCmd.Run("insertUsingDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query inserted record.
            readOutput = _spannerCmd.Run("querySingersTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("Virginia Watson", readOutput.Stdout);
            // Update record using a DML Statement.
            readOutput = _spannerCmd.Run("updateUsingDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query updated record.
            readOutput = _spannerCmd.Run("queryAlbumsTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("1 1 600000", readOutput.Stdout);
            // Delete record using a DML Statement.
            readOutput = _spannerCmd.Run("deleteUsingDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query deleted record.
            readOutput = _spannerCmd.Run("querySingersTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.DoesNotContain("Alice Trentor", readOutput.Stdout);
            // Update the timestamp of a record using a DML Statement.
            readOutput = _spannerCmd.Run("updateUsingDmlWithTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("2 row(s) updated...", readOutput.Stdout);
            // Insert a record using a DML Statement and then query the inserted record.
            readOutput = _spannerCmd.Run("writeAndReadUsingDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("Timothy Campbell", readOutput.Stdout);
            // Update record using a DML Statement combined with a Spanner struct.
            readOutput = _spannerCmd.Run("updateUsingDmlWithStruct",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query updated record.
            readOutput = _spannerCmd.Run("querySingersTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("Timothy Grant", readOutput.Stdout);
            // Insert multiple records using a DML Statement.
            readOutput = _spannerCmd.Run("writeUsingDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query inserted record.
            readOutput = _spannerCmd.Run("querySingersTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("Melissa Garcia", readOutput.Stdout);
            Assert.Contains("Russell Morales", readOutput.Stdout);
            Assert.Contains("Jacqueline Long", readOutput.Stdout);
            Assert.Contains("Dylan Shaw", readOutput.Stdout);
            // Query inserted record with a parameter.
            readOutput = _spannerCmd.Run("queryWithParameter",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("SingerId : 12 FirstName : Melissa LastName : Garcia", readOutput.Stdout);
            // Update records within a transaction using a DML Statement.
            readOutput = _spannerCmd.Run("writeWithTransactionUsingDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query updated records.
            readOutput = _spannerCmd.Run("queryAlbumsTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("1 1 800000", readOutput.Stdout);
            Assert.Contains("2 2 100000", readOutput.Stdout);
            // Update records using a partitioned DML Statement.
            readOutput = _spannerCmd.Run("updateUsingPartitionedDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query updated records.
            readOutput = _spannerCmd.Run("queryAlbumsTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("1 1 800000", readOutput.Stdout);
            Assert.Contains("2 2 100000", readOutput.Stdout);
            Assert.Contains("2 3 100000", readOutput.Stdout);
            // Delete multiple records using a partitioned DML Statement.
            readOutput = _spannerCmd.Run("deleteUsingPartitionedDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query deleted records.
            readOutput = _spannerCmd.Run("querySingersTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.DoesNotContain("Timothy Grant", readOutput.Stdout);
            Assert.DoesNotContain("Melissa Garcia", readOutput.Stdout);
            Assert.DoesNotContain("Russell Morales", readOutput.Stdout);
            Assert.DoesNotContain("Jacqueline Long", readOutput.Stdout);
            Assert.DoesNotContain("Dylan Shaw", readOutput.Stdout);
            // Insert and update a record using Batch DML.
            readOutput = _spannerCmd.Run("updateUsingBatchDml",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Query updated record.
            readOutput = _spannerCmd.Run("queryAlbumsTable",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("1 3 20000", readOutput.Stdout);
        }

        [Fact]
        void TestQueryWithArray()
        {
            // Query records using an array parameter.
            ConsoleOutput readOutput = _spannerCmd.Run("queryWithArray",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("19 Venue 19 2020-11-01", readOutput.Stdout);
            Assert.Contains("42 Venue 42 2020-10-01", readOutput.Stdout);
        }

        [Fact]
        void TestQueryWithBool()
        {
            // Query records using an bool parameter.
            ConsoleOutput readOutput = _spannerCmd.Run("queryWithBool",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("19 Venue 19 True", readOutput.Stdout);
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1062")]
        void TestQueryWithBytes()
        {
            // Query records using an bytes parameter.
            ConsoleOutput readOutput = _spannerCmd.Run("queryWithBytes",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("4 Venue 4", readOutput.Stdout);
        }

        [Fact]
        void TestQueryWithDate()
        {
            // Query records using an date parameter.
            ConsoleOutput readOutput = _spannerCmd.Run("queryWithDate",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("4 Venue 4 2018-09-02", readOutput.Stdout);
            Assert.Contains("42 Venue 42 2018-10-01", readOutput.Stdout);
        }

        [Fact]
        void TestQueryWithFloat()
        {
            // Query records using an float64 parameter.
            ConsoleOutput readOutput = _spannerCmd.Run("queryWithFloat",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("4 Venue 4 0.8", readOutput.Stdout);
            Assert.Contains("19 Venue 19 0.9", readOutput.Stdout);
        }

        [Fact]
        void TestQueryWithInt()
        {
            // Query records using an int64 parameter.
            ConsoleOutput readOutput = _spannerCmd.Run("queryWithInt",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("19 Venue 19 6300", readOutput.Stdout);
            Assert.Contains("42 Venue 42 3000", readOutput.Stdout);
        }

        [Fact]
        void TestQueryWithString()
        {
            // Query records using an string parameter.
            ConsoleOutput readOutput = _spannerCmd.Run("queryWithString",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm expected result in output.
            Assert.Contains("42 Venue 42", readOutput.Stdout);
        }

        [Fact]
        void TestQueryWithTimestamp()
        {
            // Query records using a timestamp parameter.
            ConsoleOutput readOutput = _spannerCmd.Run("queryWithTimestamp",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, readOutput.ExitCode);
            // Confirm output includes valid timestamps.
            string columnText = "LastUpdateTime : ";
            string[] result = readOutput.Stdout.Split(
                new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string valueToTest = result[0].Substring(result[0].IndexOf(columnText) + columnText.Length);
            DateTime value;
            Assert.True(DateTime.TryParse(valueToTest, out value));
        }

        [Fact(Skip = "Triggers infinite loop described here: https://github.com/commandlineparser/commandline/commit/95ded2dbcc5285302723e68221cd30a72444ba84")]
        void TestSpannerNoArgsSucceeds()
        {
            ConsoleOutput output = _spannerCmd.Run();
            Assert.NotEqual(0, output.ExitCode);
        }

        [Fact(Skip = "Skip to prevent CI test flakiness.")]
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
                    if (spannerException != null && spannerException.ErrorCode == ErrorCode.AlreadyExists)
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

        [Fact]
        void TestConnectionWithQueryOptions()
        {
            ConsoleOutput output = _spannerCmd.Run("createConnectionWithQueryOptions",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestRunCommandWithQueryOptions()
        {
            ConsoleOutput output = _spannerCmd.Run("runCommandWithQueryOptions",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.DatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
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

        [Fact]
        void TestCancelBackup()
        {
            ConsoleOutput output = _spannerCmd.Run("cancelBackupOperation",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.BackupDatabaseId,
                _fixture.ToBeCancelledBackupId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Create backup operation cancelled", output.Stdout);
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1065")]
        void TestGetBackupOperations()
        {
            ConsoleOutput output = _spannerCmd.Run("getBackupOperations",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.BackupDatabaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(_fixture.BackupId, output.Stdout);
            Assert.Contains(_fixture.BackupDatabaseId, output.Stdout);
            Assert.Contains("100% complete", output.Stdout);
        }

        [Fact]
        void TestGetBackups()
        {
            ConsoleOutput output = _spannerCmd.Run("getBackups",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.BackupDatabaseId, _fixture.BackupId);
            Assert.Equal(0, output.ExitCode);
            // BackupId should be a result of each of the 7 ListBackups calls and
            // once in a filter that is printed. But since we create a backup and
            // reuse it across runs, the filter on create_time and expire_time may not capture this
            // backup so the check is for >= 5.
            Assert.True(Regex.Matches(output.Stdout, _fixture.BackupId).Count >= 5);
        }

        [Fact]
        void TestRestoreDatabase()
        {
            ConsoleOutput output = _spannerCmd.Run("restoreDatabase",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.RestoredDatabaseId, _fixture.BackupId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(_fixture.BackupId, output.Stdout);
            Assert.Contains(_fixture.RestoredDatabaseId, output.Stdout);
            Assert.Contains($"was restored to {_fixture.RestoredDatabaseId} from backup", output.Stdout);
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1065")]
        void TestUpdateBackup()
        {
            ConsoleOutput output = _spannerCmd.Run("updateBackup",
                _fixture.ProjectId, _fixture.InstanceId, _fixture.BackupId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Updated Backup ExpireTime", output.Stdout);
        }
    }
}
