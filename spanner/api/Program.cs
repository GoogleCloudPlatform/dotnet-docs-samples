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
using System.Threading.Tasks;
using Google.Cloud.Spanner.Data;
using CommandLine;
using Google.Cloud.Spanner.Admin.Database.V1;
using System.Transactions;

namespace GoogleCloudSamples.Spanner
{
    [Verb("createDatabase", HelpText = "Create a Spanner database in your project.")]
    class CreateDatabaseOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the database will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database to create.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createSampleDatabase", HelpText = "Create a sample Spanner database along with sample tables in your project.")]
    class CreateSampleDatabaseOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the sample database to create.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createTable", HelpText = "Create a table in your project's Spanner database.")]
    class CreateTableOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the table will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the table will be created.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The name of the table to create.", Required = true)]
        public string tableName { get; set; }
    }

    [Verb("insertSampleData", HelpText = "Insert sample data into sample Spanner database table.")]
    class InsertSampleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data will be inserted.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data will be inserted.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("querySampleData", HelpText = "Query sample data from sample Spanner database table.")]
    class QuerySampleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("addIndex", HelpText = "Add an index to the sample Spanner database table.")]
    class AddIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("addStoringIndex", HelpText = "Add a storing index to the sample Spanner database table.")]
    class AddStoringIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithIndex", HelpText = "Query the sample Spanner database table using an index.")]
    class QueryDataWithIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithStoringIndex", HelpText = "Query the sample Spanner database table using an storing index.")]
    class QueryDataWithStoringIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("addColumn", HelpText = "Add a column to the sample Spanner database table.")]
    class AddColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("writeDataToNewColumn", HelpText = "Write data to a newly added column in the sample Spanner database table.")]
    class WriteDataToNewColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryNewColumn", HelpText = "Query data from a newly added column in the sample Spanner database table.")]
    class QueryNewColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithTransaction", HelpText = "Query the sample Spanner database table using a transaction.")]
    class QueryDataWithTransactionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("readWriteWithTransaction", HelpText = "Update data in the sample Spanner database table using a read-write transaction.")]
    class ReadWriteWithTransactionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    public class Program
    {
        public static async Task<object> CreateSampleDatabaseAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START create_database]
            // Create client.
            DatabaseAdminClient databaseAdminClient =
                await DatabaseAdminClient.CreateAsync();
            // Initialize request argument(s).
            InstanceName parent = new InstanceName(projectId, instanceId);
            string createStatement = $"CREATE DATABASE `{databaseId}`";
            // Make the request.
            var createDatabaseRequest =
                await databaseAdminClient.CreateDatabaseAsync(parent,
                createStatement);
            // Poll until the returned long-running operation is complete.
            var createDatabaseResponse =
                await createDatabaseRequest.PollUntilCompletedAsync();
            // Display the operation result.
            Console.WriteLine("Database create operation state:"
                + $" {createDatabaseResponse.Result.State}");
            // Define create table statements for two tables.
            string[] createTableStatements = {
               @"CREATE TABLE Singers (
                     SingerId INT64 NOT NULL,
                     FirstName    STRING(1024),
                     LastName STRING(1024),
                     ComposerInfo   BYTES(MAX)
                 ) PRIMARY KEY (SingerId)",
                 @"CREATE TABLE Albums (
                     SingerId     INT64 NOT NULL,
                     AlbumId      INT64 NOT NULL,
                     AlbumTitle   STRING(MAX)
                 ) PRIMARY KEY (SingerId, AlbumId),
                 INTERLEAVE IN PARENT Singers ON DELETE CASCADE"};
            // Make the request.
            var response =
                await databaseAdminClient.UpdateDatabaseDdlAsync(
                    new DatabaseName(projectId, instanceId,
                        databaseId), createTableStatements);
            // Poll until the returned long-running operation is complete.
            var completedResponse =
                await response.PollUntilCompletedAsync();
            // Return the operation result.
            return completedResponse.Result;
            // [END create_database]
        }

        public static async Task<object> AddIndexAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START create_index]
            // Create client.
            DatabaseAdminClient databaseAdminClient =
                await DatabaseAdminClient.CreateAsync();
            // Initialize request argument(s).
            InstanceName parent = new InstanceName(projectId, instanceId);
            string createStatement =
                $"CREATE INDEX AlbumsByAlbumTitle ON Albums(AlbumTitle)";
            // Make the request.
            var createDatabaseRequest =
                await databaseAdminClient.UpdateDatabaseDdlAsync(
                    new DatabaseName(projectId, instanceId, databaseId),
                new[] { createStatement });
            // Poll until the returned long-running operation is complete.
            var updateDatabaseResponse =
                await createDatabaseRequest.PollUntilCompletedAsync();
            Console.WriteLine("Added the AlbumsByAlbumTitle index.");
            return updateDatabaseResponse.Result;
            // [END create_index]
        }

        public static async Task<object> AddStoringIndexAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START create_storing_index]
            // Create client.
            DatabaseAdminClient databaseAdminClient =
                await DatabaseAdminClient.CreateAsync();
            // Initialize request argument(s).
            InstanceName parent = new InstanceName(projectId, instanceId);
            string createStatement =
                $"CREATE INDEX AlbumsByAlbumTitle2 ON Albums(AlbumTitle) "
                + "STORING (MarketingBudget)";
            // Make the request.
            var createDatabaseRequest =
                await databaseAdminClient.UpdateDatabaseDdlAsync(
                    new DatabaseName(projectId, instanceId, databaseId),
                new[] { createStatement });
            // Poll until the returned long-running operation is complete.
            var updateDatabaseResponse =
                await createDatabaseRequest.PollUntilCompletedAsync();
            return updateDatabaseResponse.Result;
            // [END create_storing_index]
        }

        public static async Task<object> AddColumnAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START add_column]
            // Create client.
            DatabaseAdminClient databaseAdminClient =
                await DatabaseAdminClient.CreateAsync();
            // Initialize request argument(s).
            InstanceName parent = new InstanceName(projectId, instanceId);
            string alterStatement =
                $"ALTER TABLE Albums ADD COLUMN MarketingBudget INT64";
            // Make the request.
            var createDatabaseRequest =
                await databaseAdminClient.UpdateDatabaseDdlAsync(
                    new DatabaseName(projectId, instanceId, databaseId),
                new[] { alterStatement });
            // Poll until the returned long-running operation is complete.
            var updateDatabaseResponse =
                await createDatabaseRequest.PollUntilCompletedAsync();
            Console.WriteLine("Added the MarketingBudget column.");
            return updateDatabaseResponse.Result;
            // [END add_column]
        }

        public static async Task QuerySampleDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START query_data]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create connection to spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"SingerId : {reader["SingerId"]} "
                        + $"AlbumId : {reader["AlbumId"]} "
                        + $"AlbumTitle : {reader["AlbumTitle"]}");
                    }
                }
            }
            return;
            // [END query_data]
        }


        public static async Task QueryDataWithIndexAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START query_data_with_index]
            // [START read_data_with_index]
            string startTitle = "Happy";
            string endTitle = "Super";
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT AlbumId, AlbumTitle, MarketingBudget FROM Albums@ "
                    + "{FORCE_INDEX=AlbumsByAlbumTitle} "
                    + $"WHERE AlbumTitle >= '{startTitle}' "
                    + $"AND AlbumTitle < '{endTitle}'");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"AlbumId : {reader["AlbumId"]} "
                        + $"AlbumTitle : {reader["AlbumTitle"]} "
                        + $"MarketingBudget : {reader["MarketingBudget"]}");
                    }
                }
            }
            return;
            // [END read_data_with_index]
            // [END query_data_with_index]
        }

        public static async Task QueryDataWithStoringIndexAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START read_data_with_storing_index]
            string startTitle = "Aardvark";
            string endTitle = "Goo";
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateSelectCommand(
                    "SELECT AlbumId, AlbumTitle, MarketingBudget FROM Albums@ "
                    + "{FORCE_INDEX=AlbumsByAlbumTitle2} "
                    + $"WHERE AlbumTitle >= '{startTitle}' "
                    + $"AND AlbumTitle < '{endTitle}'");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"AlbumId : {reader["AlbumId"]} "
                        + $"AlbumTitle : {reader["AlbumTitle"]} "
                        + $"MarketingBudget : {reader["MarketingBudget"]}");
                    }
                }
            }
            return;
            // [END read_data_with_storing_index]
        }

        public static async Task QueryDataWithTransactionAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START read_only_transaction]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Gets a transaction object that captures the database state
            // at a specific point in time.
            using (TransactionScope scope = new TransactionScope(
                TransactionScopeAsyncFlowOption.Enabled))
            {
                // Create connection to spanner.
                using (var connection = new SpannerConnection(
                    connectionString))
                {
                    var cmd = connection.CreateSelectCommand(
                        "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                    // Read #1.
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(
                                $"SingerId : {reader["SingerId"]}"
                                + $" AlbumId : {reader["AlbumId"]}"
                                + $" AlbumTitle : {reader["AlbumTitle"]}");
                        }
                    }
                    // Read #2. Even if changes occur in-between the reads, 
                    // the transaction ensures that Read #1 and Read #2 
                    // return the same data.
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(
                                $"SingerId : {reader["SingerId"]}"
                                + $"AlbumId : {reader["AlbumId"]} "
                                + $"AlbumTitle : {reader["AlbumTitle"]}");
                        }
                    }
                }
                // Commit the transaction to Spanner table.
                scope.Complete();
                // Yield Task thread back to the current context.
                await Task.Yield();
                return;
                // [END read_only_transaction]
            }
        }

        public static async Task ReadSampleDataAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START query_data]
            // [START read_data]
            Console.WriteLine("Querying data...");
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                Console.WriteLine("Connected...");
                var cmd = connection.CreateSelectCommand(
                    "SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"SingerId = {reader["SingerId"]} "
                        + $"AlbumId = {reader["AlbumId"]} "
                        + $"AlbumTitle = {reader["AlbumTitle"]}");
                    }
                }
            }
            return;
            // [END read_data]
            // [END query_data]
        }

        public static async Task WriteDataToNewColumnAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START update_data]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd = connection.CreateUpdateCommand("Albums",
                    new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"AlbumId", SpannerDbType.Int64},
                        {"MarketingBudget", SpannerDbType.Int64},
                    });
                var cmdLookup =
                    connection.CreateSelectCommand("SELECT * FROM Albums");
                using (var reader = await cmdLookup.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetInt64(0) == 1 && reader.GetInt64(1) == 1)
                        {
                            cmd.Parameters["SingerId"].Value =
                                reader.GetInt64(0);
                            cmd.Parameters["AlbumId"].Value =
                                reader.GetInt64(1);
                            cmd.Parameters["MarketingBudget"].Value = 100000;
                            await cmd.ExecuteNonQueryAsync();
                        }
                        if (reader.GetInt64(0) == 2 && reader.GetInt64(1) == 2)
                        {
                            cmd.Parameters["SingerId"].Value =
                                reader.GetInt64(0);
                            cmd.Parameters["AlbumId"].Value =
                                reader.GetInt64(1);
                            cmd.Parameters["MarketingBudget"].Value = 500000;
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            Console.WriteLine("Updated data.");
            return;
            // [END update_data]
        }

        public static async Task QueryNewColumnAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START query_data_with_new_column]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                var cmd =
                    connection.CreateSelectCommand("SELECT * FROM Albums");
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine($"SingerId : {reader["SingerId"]} "
                        + $"AlbumId : {reader["AlbumId"]} "
                        + $"MarketingBudget : {reader["MarketingBudget"]}");
                    }
                }
            }
            return;
            // [END query_data_with_new_column]
        }

        public static async Task ReadWriteWithTransactionAsync(
string projectId, string instanceId, string databaseId)
        {
            // [START read_write_transaction]
            // This sample transfers 200,000 from the MarketingBudget 
            // field of the second Album to the first Album. Make sure to run
            // the addColumn and writeDataToNewColumn samples first,
            // in that order.

            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";

            using (TransactionScope scope = new TransactionScope(
                TransactionScopeAsyncFlowOption.Enabled))
            {
                decimal transferAmount = 200000;
                decimal minimumAmountToTransfer = 300000;
                decimal secondBudget = 0;
                decimal firstBudget = 0;

                // Create connection to spanner.
                using (var connection =
                    new SpannerConnection(connectionString))
                {
                    // Create statement to select the second album's data.
                    var cmdLookup = connection.CreateSelectCommand(
                    "SELECT * FROM Albums WHERE SingerId = 2 AND AlbumId = 2");
                    // Excecute the select query.
                    using (var reader = await cmdLookup.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Read the second album's budget.
                            secondBudget =
                                decimal.Parse(reader["MarketingBudget"]
                                    .ToString());
                            // Confirm second Album has > 300,000 and if not
                            // raise an exception. Raising an exception will 
                            // automatically roll back the transaction.
                            if (secondBudget < minimumAmountToTransfer)
                            {
                                throw new Exception("The second album's "
                                    + $"budget {secondBudget} "
                                    + "is less than the minimum required "
                                    + "amount to transfer.");
                            }
                        }
                    }
                    // Read the first album's budget.
                    cmdLookup = connection.CreateSelectCommand(
                    "SELECT * FROM Albums WHERE SingerId = 1 and AlbumId = 1");
                    using (var reader = await cmdLookup.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            firstBudget =
                                decimal.Parse(reader["MarketingBudget"]
                                    .ToString());
                        }
                    }

                    // Specify update command parameters.
                    var cmd = connection.CreateUpdateCommand("Albums",
                        new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"AlbumId", SpannerDbType.Int64},
                        //{"AlbumTitle", SpannerDbType.String},
                        {"MarketingBudget", SpannerDbType.Int64},
                    });
                    // Update second album to remove the transfer amount.
                    secondBudget = secondBudget - transferAmount;
                    cmd.Parameters["SingerId"].Value = 2;
                    cmd.Parameters["AlbumId"].Value = 2;
                    cmd.Parameters["MarketingBudget"].Value = secondBudget;
                    await cmd.ExecuteNonQueryAsync();
                    // Update first album to add the transfer amount.
                    firstBudget = firstBudget + transferAmount;
                    cmd.Parameters["SingerId"].Value = 1;
                    cmd.Parameters["AlbumId"].Value = 1;
                    cmd.Parameters["MarketingBudget"].Value = firstBudget;
                    await cmd.ExecuteNonQueryAsync();

                    // Commit the transaction to Spanner table.
                    scope.Complete();
                    // Yield Task thread back to the current context.
                    await Task.Yield();
                    Console.WriteLine("Transaction complete.");
                    return;
                    // [END read_write_transaction]
                }
            }
        }

        public static async Task InsertSampleDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START insert_data]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                // Insert rows into the Singers table.
                var cmd = connection.CreateInsertCommand("Singers",
                    new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"FirstName", SpannerDbType.String},
                        {"LastName", SpannerDbType.String}
                    });
                // Insert first row
                cmd.Parameters["SingerId"].Value = 1;
                cmd.Parameters["FirstName"].Value = "Summerian";
                cmd.Parameters["LastName"].Value = " Chanter";
                await cmd.ExecuteNonQueryAsync();
                // Insert second row
                cmd.Parameters["SingerId"].Value = 2;
                cmd.Parameters["FirstName"].Value = "African";
                cmd.Parameters["LastName"].Value = "Artist";
                await cmd.ExecuteNonQueryAsync();
                // Insert third row
                cmd.Parameters["SingerId"].Value = 3;
                cmd.Parameters["FirstName"].Value = "Indigenous";
                cmd.Parameters["LastName"].Value = "Star";
                await cmd.ExecuteNonQueryAsync();
                // Insert rows into the Albums table.
                cmd = connection.CreateInsertCommand("Albums",
                    new SpannerParameterCollection {
                        {"SingerId", SpannerDbType.Int64},
                        {"AlbumId", SpannerDbType.Int64},
                        {"AlbumTitle", SpannerDbType.String}
                    });
                // Insert first row.
                cmd.Parameters["SingerId"].Value = 1;
                cmd.Parameters["AlbumId"].Value = 1;
                cmd.Parameters["AlbumTitle"].Value = "Hurrian Songs";
                await cmd.ExecuteNonQueryAsync();
                // Insert second row.
                cmd.Parameters["SingerId"].Value = 1;
                cmd.Parameters["AlbumId"].Value = 2;
                cmd.Parameters["AlbumTitle"].Value = "Cuneiform Chorus";
                await cmd.ExecuteNonQueryAsync();
                // Insert third row.
                cmd.Parameters["SingerId"].Value = 2;
                cmd.Parameters["AlbumId"].Value = 1;
                cmd.Parameters["AlbumTitle"].Value = "Zany Mbira Music";
                await cmd.ExecuteNonQueryAsync();
                // Insert fourth row.
                cmd.Parameters["SingerId"].Value = 2;
                cmd.Parameters["AlbumId"].Value = 2;
                cmd.Parameters["AlbumTitle"].Value = "Sistrum Sirens";
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Inserted data.");
                // Insert fifth row.
                cmd.Parameters["SingerId"].Value = 2;
                cmd.Parameters["AlbumId"].Value = 3;
                cmd.Parameters["AlbumTitle"].Value = "Congo Carols";
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Inserted data.");
                return;
            }
            // [END insert_data]
        }

        public static async Task<Database> CreateDatabaseAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START create_database]
            // Create client
            DatabaseAdminClient databaseAdminClient =
                await DatabaseAdminClient.CreateAsync();
            // Initialize request argument(s)
            InstanceName parent = new InstanceName(projectId, instanceId);
            string createStatement = $"CREATE DATABASE {databaseId}";
            // Make the request
            var response =
                await databaseAdminClient.CreateDatabaseAsync(parent,
                    createStatement);
            // Poll until the returned long-running operation is complete
            var completedResponse =
                await response.PollUntilCompletedAsync();
            // Return the operation result
            return completedResponse.Result;
            // [END create_database]
        }

        public static async Task<object> UpdateDatabaseAsync(string projectId,
            string instanceId, string databaseId, string tableName,
            string ddlStatement)
        {
            // [START update_database]
            // Create client
            DatabaseAdminClient databaseAdminClient =
                await DatabaseAdminClient.CreateAsync();

            // Make the request
            var response =
                await databaseAdminClient.UpdateDatabaseDdlAsync(
                    new DatabaseName(projectId, instanceId, databaseId),
                new[] { ddlStatement });
            // Poll until the returned long-running operation is complete
            var completedResponse =
                await response.PollUntilCompletedAsync();
            // Return the operation result
            return completedResponse.Result;
            // [END update_database]
        }

        public static object CreateSampleDatabase(string projectId,
    string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
                await CreateSampleDatabaseAsync(
                    projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Operation status: {response.Status}");
            Console.WriteLine($"Created sample database {databaseId} on "
                + $"instance {instanceId}");
            return 0;
        }

        public static object InsertSampleData(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () => await InsertSampleDataAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Operation status: {response.Status}");
            return 0;
        }

        public static object QuerySampleData(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () => await QuerySampleDataAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Operation status: {response.Status}");
            return 0;
        }

        public static object QueryDataWithIndex(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () => await QueryDataWithIndexAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Operation status: {response.Status}");
            return 0;
        }

        public static object CreateDatabase(string projectId,
            string instanceId, string databaseId)
        {
            var response = Task.Run(async () => await CreateDatabaseAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Response status: {response.Status}");
            Console.WriteLine($"Created database {databaseId} on instance "
                + $"{instanceId}");
            return 0;
        }

        public static object AddIndex(string projectId,
    string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
            await AddIndexAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Response status: {response.Status}");
            return 0;
        }
        public static object AddStoringIndex(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
            await AddStoringIndexAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Response status: {response.Status}");
            return 0;
        }

        public static object QueryDataWithStoringIndex(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
                await QueryDataWithStoringIndexAsync(
                    projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Operation status: {response.Status}");
            return 0;
        }

        public static object AddColumn(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
            await AddColumnAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Response status: {response.Status}");
            return 0;
        }

        public static object WriteDataToNewColumn(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
            await WriteDataToNewColumnAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Response status: {response.Status}");
            return 0;
        }

        public static object QueryNewColumn(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
            await QueryNewColumnAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Response status: {response.Status}");
            return 0;
        }

        public static object QueryDataWithTransaction(string projectId,
string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
                await QueryDataWithTransactionAsync(
                    projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Operation status: {response.Status}");
            return 0;
        }

        public static object ReadWriteWithTransaction(string projectId,
            string instanceId, string databaseId)
        {
            var response = Task.Run(async () =>
            await ReadWriteWithTransactionAsync(
                projectId, instanceId, databaseId));
            Console.WriteLine("Waiting for operation to complete...");
            Task.WaitAll(response);
            Console.WriteLine($"Response status: {response.Status}");
            return 0;
        }


        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                CreateDatabaseOptions, CreateSampleDatabaseOptions,
                InsertSampleDataOptions,
                QuerySampleDataOptions,
                AddColumnOptions,
                WriteDataToNewColumnOptions,
                QueryNewColumnOptions,
                QueryDataWithTransactionOptions,
                ReadWriteWithTransactionOptions,
                AddIndexOptions,
                QueryDataWithIndexOptions,
                AddStoringIndexOptions,
                QueryDataWithStoringIndexOptions
                >(args)
              .MapResult(
                (CreateSampleDatabaseOptions opts) =>
                CreateSampleDatabase(opts.projectId, opts.instanceId,
                opts.databaseId),
                (CreateDatabaseOptions opts) => CreateDatabase(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (InsertSampleDataOptions opts) => InsertSampleData(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (QuerySampleDataOptions opts) => QuerySampleData(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (AddColumnOptions opts) => AddColumn(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (WriteDataToNewColumnOptions opts) => WriteDataToNewColumn(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (QueryNewColumnOptions opts) => QueryNewColumn(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (QueryDataWithTransactionOptions opts) =>
                    QueryDataWithTransaction(
                        opts.projectId, opts.instanceId, opts.databaseId),
                (ReadWriteWithTransactionOptions opts) =>
                    ReadWriteWithTransaction(
                        opts.projectId, opts.instanceId, opts.databaseId),
                (AddIndexOptions opts) => AddIndex(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (AddStoringIndexOptions opts) => AddStoringIndex(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (QueryDataWithIndexOptions opts) => QueryDataWithIndex(
                    opts.projectId, opts.instanceId, opts.databaseId),
                (QueryDataWithStoringIndexOptions opts) =>
                    QueryDataWithStoringIndex(
                        opts.projectId, opts.instanceId, opts.databaseId),
                errs => 1);
        }
    }
}
