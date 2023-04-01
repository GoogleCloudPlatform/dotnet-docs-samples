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

// [START spanner_postgresql_interleaved_table]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Threading.Tasks;

public class CreateInterleavedTablesAsyncPostgresSample
{
    public async Task CreateInterleavedTablesAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        DatabaseAdminClient databaseAdminClient = await DatabaseAdminClient.CreateAsync();

        // The Spanner PostgreSQL dialect extends the PostgreSQL dialect with certain Spanner
        // specific features, such as interleaved tables.
        // See https://cloud.google.com/spanner/docs/postgresql/data-definition-language#create_table
        // for the full CREATE TABLE syntax.
        var ddlStatements = @"CREATE TABLE Authors 
            (AuthorId bigint NOT NULL,
            FirstName varchar(1024),
            LastName varchar(1024),
            Rating double precision,
            PRIMARY KEY (AuthorId));
            CREATE TABLE Books 
            (AuthorId bigint NOT NULL,
            BookId bigint NOT NULL,
            BookTitle text,
            PRIMARY KEY (AuthorId, BookId)) 
            INTERLEAVE IN PARENT Authors ON DELETE CASCADE;";

        DatabaseName databaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);

        // Create UpdateDatabaseRequest to create the tables. 
        var updateDatabaseRequest = new UpdateDatabaseDdlRequest
        {
            DatabaseAsDatabaseName = databaseName,
            Statements = { ddlStatements }
        };

        var updateOperation = await databaseAdminClient.UpdateDatabaseDdlAsync(updateDatabaseRequest);
        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the interleaved tables to be created.");
        var updateResponse = await updateOperation.PollUntilCompletedAsync();
        if (updateResponse.IsFaulted)
        {
            Console.WriteLine($"Error while updating database: {updateResponse.Exception}");
            throw updateResponse.Exception;
        }

        Console.WriteLine($"Created interleaved table hierarchy using PostgreSQL dialect.");
    }
}
// [END spanner_postgresql_interleaved_table]
