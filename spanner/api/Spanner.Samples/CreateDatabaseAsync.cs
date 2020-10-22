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

        using (var connection = new SpannerConnection(connectionString))
        {
            string createStatement = $"CREATE DATABASE `{databaseId}`";
            using var createDbCommand = connection.CreateDdlCommand(createStatement);
            await createDbCommand.ExecuteNonQueryAsync();
        }

        // Update connection string with Database ID for table creation.
        connectionString += $"/databases/{databaseId}";
        using (var connection = new SpannerConnection(connectionString))
        {
            // Define create table statement for table #1.
            string createTableStatement =
            @"CREATE TABLE Singers (
                     SingerId INT64 NOT NULL,
                     FirstName STRING(1024),
                     LastName STRING(1024),
                     ComposerInfo BYTES(MAX)
                 ) PRIMARY KEY (SingerId)";

            using var createTableCommand = connection.CreateDdlCommand(createTableStatement);
            await createTableCommand.ExecuteNonQueryAsync();

            // Define create table statement for table #2.
            createTableStatement =
            @"CREATE TABLE Albums (
                     SingerId INT64 NOT NULL,
                     AlbumId INT64 NOT NULL,
                     AlbumTitle STRING(MAX)
                 ) PRIMARY KEY (SingerId, AlbumId),
                 INTERLEAVE IN PARENT Singers ON DELETE CASCADE";

            using var createTableCommand1 = connection.CreateDdlCommand(createTableStatement);
            await createTableCommand1.ExecuteNonQueryAsync();
        }
    }
}
// [END spanner_create_database]
