// Copyright 2021 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START spanner_create_database_with_default_leader]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Threading.Tasks;

public class CreateDatabaseWithDefaultLeaderAsyncSample
{
    public async Task<Database> CreateDatabaseWithDefaultLeaderAsync(string projectId, string instanceId, string databaseId, string defaultLeader)
    {
        DatabaseAdminClient databaseAdminClient = await DatabaseAdminClient.CreateAsync();
        // Define create table statement for table #1.
        var createSingersTable =
            @"CREATE TABLE Singers (
                SingerId INT64 NOT NULL,
                FirstName STRING(1024),
                LastName STRING(1024),
                ComposerInfo BYTES(MAX)
            ) PRIMARY KEY (SingerId)";
        // Define create table statement for table #2.
        var createAlbumsTable =
            @"CREATE TABLE Albums (
                SingerId INT64 NOT NULL,
                AlbumId INT64 NOT NULL,
                AlbumTitle STRING(MAX)
            ) PRIMARY KEY (SingerId, AlbumId),
            INTERLEAVE IN PARENT Singers ON DELETE CASCADE";

        // Define alter database statement to set default leader.
        var alterDatabaseStatement = @$"ALTER DATABASE `{databaseId}` SET OPTIONS (default_leader = '{defaultLeader}')";

        // Create the CreateDatabase request with default leader and execute it.
        var request = new CreateDatabaseRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
            CreateStatement = $"CREATE DATABASE `{databaseId}`",
            ExtraStatements = { createSingersTable, createAlbumsTable, alterDatabaseStatement },
        };
        var operation = await databaseAdminClient.CreateDatabaseAsync(request);

        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the operation to finish.");
        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating database: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        var database = completedResponse.Result;
        Console.WriteLine($"Created database [{databaseId}]");
        Console.WriteLine($"\t Default leader: {database.DefaultLeader}");
        return database;
    }
}
// [END spanner_create_database_with_default_leader]
