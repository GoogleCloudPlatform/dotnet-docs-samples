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
using System.Transactions;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace GoogleCloudSamples.Spanner
{
    [Verb("createDatabase", HelpText = "Create a Cloud Spanner database in your project.")]
    class CreateDatabaseOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the database will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database to create.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createSampleDatabase", HelpText = "Create a sample Cloud Spanner database along with sample tables in your project.")]
    class CreateSampleDatabaseOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the sample database to create.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("createTable", HelpText = "Create a table in your project's Cloud Spanner database.")]
    class CreateTableOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when creating Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the table will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the table will be created.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The name of the table to create.", Required = true)]
        public string tableName { get; set; }
    }

    [Verb("insertSampleData", HelpText = "Insert sample data into sample Cloud Spanner database table.")]
    class InsertSampleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data will be inserted.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data will be inserted.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("querySampleData", HelpText = "Query sample data from sample Cloud Spanner database table.")]
    class QuerySampleDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("addIndex", HelpText = "Add an index to the sample Cloud Spanner database table.")]
    class AddIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("addStoringIndex", HelpText = "Add a storing index to the sample Cloud Spanner database table.")]
    class AddStoringIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithIndex", HelpText = "Query the sample Cloud Spanner database table using an index.")]
    class QueryDataWithIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The start of the title index.", Required = false)]
        public string startTitle { get; set; }
        [Value(4, HelpText = "The end of the title index.", Required = false)]
        public string endTitle { get; set; }
    }

    [Verb("queryDataWithStoringIndex", HelpText = "Query the sample Cloud Spanner database table using an storing index.")]
    class QueryDataWithStoringIndexOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.", Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The start of the title index.", Required = false)]
        public string startTitle { get; set; }
        [Value(4, HelpText = "The end of the title index.", Required = false)]
        public string endTitle { get; set; }
    }

    [Verb("addColumn", HelpText = "Add a column to the sample Cloud Spanner database table.")]
    class AddColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("writeDataToNewColumn", HelpText = "Write data to a newly added column in the sample Cloud Spanner database table.")]
    class WriteDataToNewColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryNewColumn", HelpText = "Query data from a newly added column in the sample Cloud Spanner database table.")]
    class QueryNewColumnOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("queryDataWithTransaction", HelpText = "Query the sample Cloud Spanner database table using a transaction.")]
    class QueryDataWithTransactionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("readWriteWithTransaction", HelpText = "Update data in the sample Cloud Spanner database table using a read-write transaction.")]
    class ReadWriteWithTransactionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.", Required = true)]
        public string databaseId { get; set; }
    }

    public class Program
    {
        public static async Task CreateSampleDatabaseAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START create_database]
            // Initialize request connection string for database creation.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                string createStatement = $"CREATE DATABASE `{databaseId}`";
                var cmd = connection.CreateDdlCommand(createStatement);
                await cmd.ExecuteNonQueryAsync();
            }
            // Update connection string with Database ID for table creation.
            connectionString = connectionString + $"/databases/{databaseId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                // Define create table statement for table #1.
                string createTableStatement =
               @"CREATE TABLE Singers (
                     SingerId INT64 NOT NULL,
                     FirstName    STRING(1024),
                     LastName STRING(1024),
                     ComposerInfo   BYTES(MAX)
                 ) PRIMARY KEY (SingerId)";
                // Make the request.
                var cmd = connection.CreateDdlCommand(createTableStatement);
                await cmd.ExecuteNonQueryAsync();
                // Define create table statement for table #2.
                createTableStatement =
                @"CREATE TABLE Albums (
                     SingerId     INT64 NOT NULL,
                     AlbumId      INT64 NOT NULL,
                     AlbumTitle   STRING(MAX)
                 ) PRIMARY KEY (SingerId, AlbumId),
                 INTERLEAVE IN PARENT Singers ON DELETE CASCADE";
                // Make the request.
                cmd = connection.CreateDdlCommand(createTableStatement);
                await cmd.ExecuteNonQueryAsync();
            }
            return;
            // [END create_database]
        }

        public static async Task AddIndexAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START create_index]
            // Initialize request argument(s).
            string connectionString =
                $"Data Source=projects/{projectId}/instances/"
                + $"{instanceId}/databases/{databaseId}";
            string createStatement =
                $"CREATE INDEX AlbumsByAlbumTitle ON Albums(AlbumTitle)";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                var createCmd = connection.CreateDdlCommand(createStatement);
                await createCmd.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Added the AlbumsByAlbumTitle index.");
            return;
            // [END create_index]
        }

        public static async Task AddStoringIndexAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START create_storing_index]
            // Initialize request argument(s).
            string connectionString =
                $"Data Source=projects/{projectId}/instances/"
                + $"{instanceId}/databases/{databaseId}";
            string createStatement =
                $"CREATE INDEX AlbumsByAlbumTitle2 ON Albums(AlbumTitle) "
                + "STORING (MarketingBudget)";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                var createCmd = connection.CreateDdlCommand(createStatement);
                await createCmd.ExecuteNonQueryAsync();
            }
            return;
            // [END create_storing_index]
        }

        public static async Task AddColumnAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START add_column]
            // Initialize request argument(s).
            string connectionString =
                $"Data Source=projects/{projectId}/instances/"
                + $"{instanceId}/databases/{databaseId}";
            string createStatement =
                $"ALTER TABLE Albums ADD COLUMN MarketingBudget INT64";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                var createCmd = connection.CreateDdlCommand(createStatement);
                await createCmd.ExecuteNonQueryAsync();
            }
            Console.WriteLine("Added the MarketingBudget column.");
            return;
            // [END add_column]
        }

        public static async Task QuerySampleDataAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START query_data]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create connection to Cloud Spanner.
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
            string projectId, string instanceId, string databaseId,
            string startTitle = "Aardvark", string endTitle = "Goo")
        {
            Console.WriteLine("Running QueryDataWithIndex...");
            // [START query_data_with_index]
            // [START read_data_with_index]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
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
            string projectId, string instanceId, string databaseId,
            string startTitle = "Aardvark", string endTitle = "Goo")
        {
            // [START read_data_with_storing_index]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
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
                // Create connection to Cloud Spanner.
                var connection = new SpannerConnection(
                    connectionString);
                // Open connection as read only within the scope 
                // of the current transaction.
                connection.OpenAsReadOnly();
                using (connection)
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
                                + $" AlbumId : {reader["AlbumId"]}"
                                + $" AlbumTitle : {reader["AlbumTitle"]}");
                        }
                    }
                }
                //scope.Complete();
                return;
                // [END read_only_transaction]
                // TODO - Unncomment the above line containing scope.Complete().
                // A pending client library update will enable this.
            }
        }

        public static async Task WriteDataToNewColumnAsync(
    string projectId, string instanceId, string databaseId)
        {
            // [START update_data]
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";
            // Create connection to Cloud Spanner.
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
            // Create connection to Cloud Spanner.
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

        internal class CustomTransientErrorDetectionStrategy
            : ITransientErrorDetectionStrategy
        {
            public bool IsTransient(Exception ex)
            {
                if (ex.IsTransientSpannerFault())
                    return true;
                return false;
            }
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

                // Create connection to Cloud Spanner.
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
                    scope.Complete();
                    // Yield Task thread back to the current context.
                    await Task.Yield();
                    Console.WriteLine("Transaction complete.");
                    return;
                    // [END read_write_transaction]
                    // TODO - Remove the above Task.Yield() statement. 
                    // A pending client library update will not require this
                    // for transactions.
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
            // Create connection to Cloud Spanner.
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
                cmd.Parameters["AlbumTitle"].Value = "Good Hurrian Songs";
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
                cmd.Parameters["AlbumTitle"].Value = "Ambient Sistrum Sirens";
                await cmd.ExecuteNonQueryAsync();
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

        public static async Task CreateDatabaseAsync(
            string projectId, string instanceId, string databaseId)
        {
            // [START create_custom_database]
            // Initialize request connection string for database creation.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}";
            // Make the request.
            using (var connection = new SpannerConnection(connectionString))
            {
                string createStatement = $"CREATE DATABASE `{databaseId}`";
                var cmd = connection.CreateDdlCommand(createStatement);
                await cmd.ExecuteNonQueryAsync();
            }
            return;
            // [END create_custom_database]
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
            string instanceId, string databaseId,
            string startTitle, string endTitle)
        {
            var response = new Task(() => { });
            // Call method QueryDataWithIndexAsync() based on whether 
            // user provided startTitle and endTitle values are empty or null.
            if (string.IsNullOrEmpty(startTitle))
            {
                // startTitle not provided, exclude startTitle and endTitle from 
                // method call to use default values for both.
                response = Task.Run(async () => await QueryDataWithIndexAsync(
                        projectId, instanceId, databaseId));
            }
            else if (!string.IsNullOrEmpty(startTitle)
                && string.IsNullOrEmpty(endTitle))
            {
                // Only startTitle provided, show message that user must 
                // provide both startTitle and endTitle or exclude them 
                // from their command.
                Console.WriteLine("Please provide values for both the start "
                    + "of the title index and the end of the title index.");
                Console.WriteLine("Or you can exclude both to run the "
                    + "query with default values.");
                return 0;
            }
            else if (!string.IsNullOrEmpty(startTitle)
                && !string.IsNullOrEmpty(endTitle))
            {
                // Both startTitle and endTitle provided, include them both in
                // the method call to QueryDataWithIndexAsync.
                response = Task.Run(async () => await QueryDataWithIndexAsync(
                        projectId, instanceId, databaseId,
                        startTitle, endTitle));
            }
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
            string instanceId, string databaseId, string startTitle, string endTitle)
        {
            var response = new Task(() => { });
            // Call method QueryDataWithStoringIndexAsync() based on whether 
            // user provided startTitle and endTitle values are empty or null.
            if (string.IsNullOrEmpty(startTitle))
            {
                // startTitle not provided, exclude startTitle and endTitle from 
                // method call to use default values for both.
                response = Task.Run(async () =>
                    await QueryDataWithStoringIndexAsync(
                        projectId, instanceId, databaseId));
            }
            else if (!string.IsNullOrEmpty(startTitle)
                && string.IsNullOrEmpty(endTitle))
            {
                // Only startTitle provided, show message that user must 
                // provide both startTitle and endTitle or exclude them 
                // from their command.
                Console.WriteLine("Please provide values for both the start "
                    + "of the title index and the end of the title index.");
                Console.WriteLine("Or you can exclude both to run the "
                    + "query with default values.");
                return 0;
            }
            else if (!string.IsNullOrEmpty(startTitle)
                && !string.IsNullOrEmpty(endTitle))
            {
                // Both startTitle and endTitle provided, include them both in
                // the method call to QueryDataWithIndexAsync.
                response = Task.Run(async () =>
                    await QueryDataWithStoringIndexAsync(
                        projectId, instanceId, databaseId,
                        startTitle, endTitle));
            }
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
            try
            {
                int retryCount = 3;
                var retryStrategy = new Incremental(retryCount, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                var retryPolicy = new RetryPolicy<CustomTransientErrorDetectionStrategy>(retryStrategy);
                var response = retryPolicy.ExecuteAction(async () => await ReadWriteWithTransactionAsync(projectId, instanceId, databaseId));
                Console.WriteLine("Waiting for operation to complete...");
                Task.WaitAll(response);
                Console.WriteLine($"Response status: {response.Status}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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
                    opts.projectId, opts.instanceId, opts.databaseId,
                    opts.startTitle, opts.endTitle),
                (QueryDataWithStoringIndexOptions opts) =>
                    QueryDataWithStoringIndex(
                        opts.projectId, opts.instanceId, opts.databaseId,
                        opts.startTitle, opts.endTitle),
                errs => 1);
        }
    }
}
