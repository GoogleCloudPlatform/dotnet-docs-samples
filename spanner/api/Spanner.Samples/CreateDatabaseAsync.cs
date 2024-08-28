// Copyright 2020 Google Inc.
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

// [START spanner_create_database]

using Google.Cloud.Spanner.Data;
using System.Threading.Tasks;

public class CreateDatabaseAsyncSample
{
    public async Task CreateDatabaseAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}";

        using var connection = new SpannerConnection(connectionString);
        var createDatabase = $"CREATE DATABASE `{databaseId}`";
        // Define create table statement for table #1.
        var createSingersTable =
            @"CREATE TABLE Singers (
                SingerId INT64 NOT NULL,
                FirstName STRING(1024),
                LastName STRING(1024),
                ComposerInfo BYTES(MAX),
                FullName STRING(2048) AS (ARRAY_TO_STRING([FirstName, LastName], "" "")) STORED
            ) PRIMARY KEY (SingerId)";
        // Define create table statement for table #2.
        var createAlbumsTable =
            @"CREATE TABLE Albums (
                SingerId INT64 NOT NULL,
                AlbumId INT64 NOT NULL,
                AlbumTitle STRING(MAX)
            ) PRIMARY KEY (SingerId, AlbumId),
            INTERLEAVE IN PARENT Singers ON DELETE CASCADE";

        using var createDbCommand = connection.CreateDdlCommand(createDatabase, createSingersTable, createAlbumsTable);
        await createDbCommand.ExecuteNonQueryAsync();
    }
}
// [END spanner_create_database]
