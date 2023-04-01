// Copyright 2022 Google Inc.
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

// [START spanner_postgresql_create_database]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Threading.Tasks;

public class CreateDatabaseAsyncPostgresSample
{
    public async Task CreateDatabaseAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        DatabaseAdminClient databaseAdminClient = await DatabaseAdminClient.CreateAsync();

        // Create the CreateDatabaseRequest with PostgreSQL dialect and execute it.
        // There cannot be Extra DDL statements while creating PostgreSQL.
        var createDatabaseRequest = new CreateDatabaseRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
            CreateStatement = $"CREATE DATABASE \"{databaseId}\"",
            DatabaseDialect = DatabaseDialect.Postgresql
        };

        var createOperation = await databaseAdminClient.CreateDatabaseAsync(createDatabaseRequest);

        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the database to be created.");
        var completedResponse = await createOperation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating PostgreSQL database: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        // PostgreSQL Database is created. Now, we can create the tables.
        // Define create table statement for table #1 in PostgreSQL syntax.
        var createSingersTable = @"CREATE TABLE Singers (
            SingerId bigint NOT NULL PRIMARY KEY,
            FirstName varchar(1024),
            LastName varchar(1024),
            Rating numeric,
            SingerInfo bytea,
            FullName character varying(2048) GENERATED ALWAYS AS (FirstName || ' ' || LastName) STORED)";

        // Define create table statement for table #2 in PostgreSQL syntax.
        var createAlbumsTable = @"CREATE TABLE Albums (
            AlbumId bigint NOT NULL PRIMARY KEY,
            SingerId bigint NOT NULL REFERENCES Singers (SingerId),
            AlbumTitle text,
            MarketingBudget BIGINT)";

        DatabaseName databaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);

        // Create UpdateDatabaseRequest to create the tables. 
        var updateDatabaseRequest = new UpdateDatabaseDdlRequest
        {
            DatabaseAsDatabaseName = databaseName,
            Statements = { createSingersTable, createAlbumsTable }
        };

        var updateOperation = await databaseAdminClient.UpdateDatabaseDdlAsync(updateDatabaseRequest);
        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the tables to be created.");
        var updateResponse = await updateOperation.PollUntilCompletedAsync();
        if (updateResponse.IsFaulted)
        {
            Console.WriteLine($"Error while updating database: {updateResponse.Exception}");
            throw updateResponse.Exception;
        }
    }
}
// [END spanner_postgresql_create_database]
