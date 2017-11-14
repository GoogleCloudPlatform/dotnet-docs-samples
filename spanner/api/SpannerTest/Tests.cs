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
using Xunit;
using Xunit.Sdk;

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

    public class SpannerTests
    {
        private static readonly string s_projectId =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        // Allow environment variables to override the default instance and database names.
        private static readonly string s_instanceId =
            Environment.GetEnvironmentVariable("TEST_SPANNER_INSTANCE") ?? "my-instance";
        private static readonly string s_databaseId =
            Environment.GetEnvironmentVariable("TEST_SPANNER_DATABASE") ?? "my-database";
        private static bool s_initializedDatabase = false;

        readonly CommandLineRunner _spannerCmd = new CommandLineRunner()
        {
            Main = Program.Main,
            Command = "Spanner"
        };

        public SpannerTests()
        {
            lock (this)
            {
                if (!s_initializedDatabase)
                {
                    s_initializedDatabase = true;
                    InitializeDatabase();
                }
            }
        }

        void InitializeDatabase()
        {
            // If the database has not been initialized, retry.
            _spannerCmd.Run("dropSampleTables",
                s_projectId, s_instanceId, s_databaseId);
            _spannerCmd.Run("createSampleDatabase",
                s_projectId, s_instanceId, s_databaseId);
            _spannerCmd.Run("insertSampleData",
                s_projectId, s_instanceId, s_databaseId);
            _spannerCmd.Run("addColumn",
                s_projectId, s_instanceId, s_databaseId);
        }

        async Task RefillMarketingBudgetsAsync(int firstAlbumBudget,
            int secondAlbumBudget)
        {
            string connectionString =
                $"Data Source=projects/{s_projectId}/instances/{s_instanceId}"
                + $"/databases/{s_databaseId}";

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
                s_projectId, s_instanceId, s_databaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestQueryTransactionCore()
        {
            ConsoleOutput output = _spannerCmd.Run("queryDataWithTransaction",
                s_projectId, s_instanceId, s_databaseId, "netcore");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }

        [Fact]
        void TestReadWriteTransaction()
        {
            RefillMarketingBudgetsAsync(300000, 300000).Wait();
            ConsoleOutput output = _spannerCmd.Run("readWriteWithTransaction",
                s_projectId, s_instanceId, s_databaseId);
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
                    s_projectId, s_instanceId, s_databaseId);
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
                s_projectId, s_instanceId, s_databaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Go, Go, Go", output.Stdout);
        }

        [Fact]
        void TestReadWriteTransactionCore()
        {
            RefillMarketingBudgetsAsync(300000, 300000).Wait();
            ConsoleOutput output = _spannerCmd.Run("readWriteWithTransaction",
                s_projectId, s_instanceId, s_databaseId, "netcore");
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
                    s_projectId, s_instanceId, s_databaseId, "netcore");
            }
            catch (Exception e)
            {
                Assert.Contains("less than the minimum required amount",
                e.ToString());
            }
        }

        [Fact]
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
                        s_projectId, s_instanceId, s_databaseId);
            }
            catch (AggregateException e)
            {
                bool rethrow = true;
                foreach (var innerException in e.InnerExceptions)
                {
                    SpannerException spannerException = innerException as SpannerException;
                    if (spannerException != null && spannerException.Message.ToLower().Contains("duplicate"))
                    {
                        Console.WriteLine($"Database {s_databaseId} already exists.");
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
                    s_projectId, s_instanceId, s_databaseId);
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
                s_projectId, s_instanceId, s_databaseId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("SingerId : 1 AlbumId : 1", output.Stdout);
            Assert.Contains("SingerId : 2 AlbumId : 1", output.Stdout);
        }
    }
}
