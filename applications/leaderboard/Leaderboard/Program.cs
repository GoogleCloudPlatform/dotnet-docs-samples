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

namespace GoogleCloudSamples.Leaderboard
{
    [Verb("create", HelpText = "Create a sample Cloud Spanner database "
        + "along with sample 'Players' and 'Scores' tables in your project.")]
    class CreateOptions
    {
        [Value(0, HelpText = "The project ID of the project to use "
            + "when creating Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database "
            + "will be created.", Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the sample database to create.",
            Required = true)]
        public string databaseId { get; set; }
    }

    [Verb("insert", HelpText = "Insert sample 'players' records or 'scores' records "
        + "into the database.")]
    class InsertOptions
    {
        [Value(0, HelpText = "The project ID of the project to use "
            + "when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.",
            Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample database resides.",
            Required = true)]
        public string databaseId { get; set; }
        [Value(3, HelpText = "The type of insert to perform, 'players' or 'scores'.",
            Required = true)]
        public string insertType { get; set; }
    }

    [Verb("query", HelpText = "Query players with 'Top Ten' scores within a specific timespan "
        + "from sample Cloud Spanner database table.")]
    class QueryOptions
    {
        [Value(0, HelpText = "The project ID of the project to use "
            + "when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample data resides.",
            Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database where the sample data resides.",
            Required = true)]
        public string databaseId { get; set; }
        [Value(3, Default = 0, HelpText = "The timespan in hours that will be used to filter the "
            + "results based on a record's timestamp. The default will return the "
            + "'Top Ten' scores of all time.")]
        public int timespan { get; set; }
    }

    [Verb("delete", HelpText = "Delete a Spanner database.")]
    class DeleteOptions
    {
        [Value(0, HelpText = "The project ID of the project to use "
            + "when managing Cloud Spanner resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The ID of the instance where the sample database resides.",
            Required = true)]
        public string instanceId { get; set; }
        [Value(2, HelpText = "The ID of the database to delete.", Required = true)]
        public string databaseId { get; set; }
    }

    public class Program
    {
        enum ExitCode : int
        {
            Success = 0,
            InvalidParameter = 1,
        }

        public static object Create(string projectId,
            string instanceId, string databaseId)
        {
            var response =
                CreateAsync(projectId, instanceId, databaseId);
            Console.WriteLine("Waiting for operation to complete...");
            response.Wait();
            Console.WriteLine($"Operation status: {response.Status}");
            Console.WriteLine($"Created sample database {databaseId} on "
                + $"instance {instanceId}");
            return ExitCode.Success;
        }

        public static async Task CreateAsync(
            string projectId, string instanceId, string databaseId)
        {
            // Initialize request connection string for database creation.
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}";
            using (var connection = new SpannerConnection(connectionString))
            {
                string createStatement = $"CREATE DATABASE `{databaseId}`";
                string[] createTableStatements = new string[] {
                  // Define create table statement for Players table.
                  @"CREATE TABLE Players(
                    PlayerId INT64 NOT NULL,
                    PlayerName STRING(2048) NOT NULL
                  ) PRIMARY KEY(PlayerId)",
                  // Define create table statement for Scores table.
                  @"CREATE TABLE Scores(
                    PlayerId INT64 NOT NULL,
                    Score INT64 NOT NULL,
                    Timestamp TIMESTAMP NOT NULL OPTIONS(allow_commit_timestamp=true)
                  ) PRIMARY KEY(PlayerId, Timestamp),
                      INTERLEAVE IN PARENT Players ON DELETE NO ACTION" };
                // Make the request.
                var cmd = connection.CreateDdlCommand(
                    createStatement, createTableStatements);
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (SpannerException e) when
                    (e.ErrorCode == ErrorCode.AlreadyExists)
                {
                    // OK.
                }
            }
        }

        public static object Insert(string projectId,
            string instanceId, string databaseId, string insertType)
        {
            if (insertType.ToLower() == "players")
            {
                var responseTask =
                    InsertPlayersAsync(projectId, instanceId, databaseId);
                Console.WriteLine("Waiting for insert players operation to complete...");
                responseTask.Wait();
                Console.WriteLine($"Operation status: {responseTask.Status}");
            }
            else if (insertType.ToLower() == "scores")
            {
                var responseTask =
                    InsertScoresAsync(projectId, instanceId, databaseId);
                Console.WriteLine("Waiting for insert scores operation to complete...");
                responseTask.Wait();
                Console.WriteLine($"Operation status: {responseTask.Status}");
            }
            else
            {
                Console.WriteLine("Invalid value for 'type of insert'. "
                    + "Specify 'players' or 'scores'.");
                return ExitCode.InvalidParameter;
            }
            Console.WriteLine($"Inserted {insertType} into sample database "
                + $"{databaseId} on instance {instanceId}");
            return ExitCode.Success;
        }

        public static async Task InsertPlayersAsync(string projectId,
            string instanceId, string databaseId)
        {
            string connectionString =
                $"Data Source=projects/{projectId}/instances/{instanceId}"
                + $"/databases/{databaseId}";
            Int64 numberOfPlayers = 0;
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var tx = await connection.BeginTransactionAsync())
                {
                    // Execute a SQL statement to get current number of records
                    // in the Players table.
                    var cmd = connection.CreateSelectCommand(
                        @"SELECT Count(PlayerId) as PlayerCount FROM Players");
                    cmd.Transaction = tx;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long parsedValue;
                            if (reader["PlayerCount"] != DBNull.Value)
                            {
                                bool result = Int64.TryParse(
                                    reader.GetFieldValue<string>("PlayerCount"),
                                        out parsedValue);
                                if (result)
                                {
                                    numberOfPlayers = parsedValue;
                                }
                            }
                        }
                    }
                    // Insert 100 player records into the Players table.
                    using (cmd = connection.CreateInsertCommand(
                        "Players", new SpannerParameterCollection
                    {
                        { "PlayerId", SpannerDbType.String },
                        { "PlayerName", SpannerDbType.String }
                    }))
                    {
                        cmd.Transaction = tx;
                        for (var x = 1; x <= 100; x++)
                        {
                            numberOfPlayers++;
                            cmd.Parameters["PlayerId"].Value =
                                Math.Abs(Guid.NewGuid().GetHashCode());
                            cmd.Parameters["PlayerName"].Value =
                                $"Player {numberOfPlayers}";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    await tx.CommitAsync();
                }
            }
            Console.WriteLine("Done inserting player records...");
        }

        public static async Task InsertScoresAsync(
            string projectId, string instanceId, string databaseId)
        {
            string connectionString =
            $"Data Source=projects/{projectId}/instances/{instanceId}"
            + $"/databases/{databaseId}";

            // Insert 4 score records into the Scores table for each player in the Players table.
            using (var connection = new SpannerConnection(connectionString))
            {
                await connection.OpenAsync();
                Random r = new Random();
                bool playerRecordsFound = false;
                var cmdLookup = connection.CreateSelectCommand("SELECT * FROM Players");
                using (var reader = await cmdLookup.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (!playerRecordsFound)
                        {
                            playerRecordsFound = true;
                        }
                        using (var tx = await connection.BeginTransactionAsync())
                        using (var cmd = connection.CreateInsertCommand(
                            "Scores", new SpannerParameterCollection
                        {
                            { "PlayerId", SpannerDbType.String },
                            { "Score", SpannerDbType.Int64 },
                            { "Timestamp", SpannerDbType.Timestamp }
                        }))
                        {
                            cmd.Transaction = tx;
                            for (var x = 1; x <= 4; x++)
                            {
                                DateTime randomTimestamp = DateTime.Now
                                    .AddYears(r.Next(-2, 1))
                                    .AddMonths(r.Next(-12, 1))
                                    .AddDays(r.Next(-10, 1))
                                    .AddSeconds(r.Next(-60, 0))
                                    .AddMilliseconds(r.Next(-100000, 0));
                                cmd.Parameters["PlayerId"].Value =
                                    reader.GetFieldValue<int>("PlayerId");
                                // Insert random score value between 10000 and 1000000.
                                cmd.Parameters["Score"].Value = r.Next(1000, 1000001);
                                // Insert random past timestamp
                                // value into Timestamp column.
                                cmd.Parameters["Timestamp"].Value =
                                    randomTimestamp.ToString("o");
                                cmd.ExecuteNonQuery();
                            }
                            await tx.CommitAsync();
                        }
                    }
                    if (!playerRecordsFound)
                    {
                        Console.WriteLine("Parameter 'scores' is invalid since "
                        + "no player records currently exist. First insert players "
                        + "then insert scores.");
                        Environment.Exit((int)ExitCode.InvalidParameter);
                    }
                    else
                    {
                        Console.WriteLine("Done inserting score records...");
                    }
                }
            }
        }

        public static object Query(string projectId,
            string instanceId, string databaseId, int timespan)
        {
            var response = QueryAsync(
                projectId, instanceId, databaseId, timespan);
            response.Wait();
            return ExitCode.Success;
        }

        public static async Task QueryAsync(
            string projectId, string instanceId, string databaseId, int timespan)
        {
            string connectionString =
            $"Data Source=projects/{projectId}/instances/"
            + $"{instanceId}/databases/{databaseId}";
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                string sqlCommand;
                if (timespan == 0)
                {
                    // No timespan specified. Query Top Ten scores of all time.
                    sqlCommand =
                        @"SELECT p.PlayerId, p.PlayerName, s.Score, s.Timestamp
                            FROM Players p
                            JOIN Scores s ON p.PlayerId = s.PlayerId
                            ORDER BY s.Score DESC LIMIT 10";
                }
                else
                {
                    // Query Top Ten scores filtered by the timepan specified.
                    sqlCommand =
                        $@"SELECT p.PlayerId, p.PlayerName, s.Score, s.Timestamp
                            FROM Players p
                            JOIN Scores s ON p.PlayerId = s.PlayerId
                            WHERE s.Timestamp >
                            TIMESTAMP_SUB(CURRENT_TIMESTAMP(),
                                INTERVAL {timespan.ToString()} HOUR)
                            ORDER BY s.Score DESC LIMIT 10";
                }
                var cmd = connection.CreateSelectCommand(sqlCommand);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("PlayerId : "
                          + reader.GetFieldValue<string>("PlayerId")
                          + " PlayerName : "
                          + reader.GetFieldValue<string>("PlayerName")
                          + " Score : "
                          + string.Format("{0:n0}",
                            Int64.Parse(reader.GetFieldValue<string>("Score")))
                          + " Timestamp : "
                          + reader.GetFieldValue<string>("Timestamp").Substring(0, 10));
                    }
                }
            }
        }

        public static object Delete(string projectId,
            string instanceId, string databaseId)
        {
            var response =
                DeleteAsync(projectId, instanceId, databaseId);
            Console.WriteLine("Waiting for operation to complete...");
            response.Wait();
            Console.WriteLine($"Operation status: {response.Status}");
            Console.WriteLine($"Deleted sample database {databaseId} on "
                + $"instance {instanceId}");
            return ExitCode.Success;
        }

        private static async Task DeleteAsync(string projectId,
            string instanceId, string databaseId)
        {
            string AdminConnectionString = $"Data Source=projects/{projectId}/"
                + $"instances/{instanceId}";
            using (var connection = new SpannerConnection(AdminConnectionString))
            using (var cmd = connection.CreateDdlCommand(
                $@"DROP DATABASE {databaseId}"))
            {
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
            Console.WriteLine($"Done deleting database: {databaseId}");
        }

        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            verbMap
                .Add((CreateOptions opts) => Create(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .Add((InsertOptions opts) => Insert(
                    opts.projectId, opts.instanceId, opts.databaseId, opts.insertType))
                .Add((QueryOptions opts) => Query(
                    opts.projectId, opts.instanceId, opts.databaseId, opts.timespan))
                .Add((DeleteOptions opts) => Delete(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }
    }
}