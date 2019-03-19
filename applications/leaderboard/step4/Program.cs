/*
 * Copyright (c) 2019 Google LLC
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

        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            verbMap
                .Add((CreateOptions opts) => Create(
                    opts.projectId, opts.instanceId, opts.databaseId))
                .NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }
    }
}