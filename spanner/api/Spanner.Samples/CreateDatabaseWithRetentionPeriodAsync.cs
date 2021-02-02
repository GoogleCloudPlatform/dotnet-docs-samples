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

// [START spanner_create_database_with_version_retention_period]

using Google.Cloud.Spanner.Data;
using System.Threading.Tasks;

public class CreateDatabaseWithRetentionPeriodAsyncSample
{
    public async Task CreateDatabaseWithRetentionPeriodAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}";

        using var connection = new SpannerConnection(connectionString);
        var createStatement = $"CREATE DATABASE `{databaseId}`";
        var versionRetentionPeriod = "7d";
        using var createDbCommand = connection.CreateDdlCommand(
            createStatement,
            @"CREATE TABLE Singers (
                         SingerId INT64 NOT NULL,
                         FirstName STRING(1024),
                         LastName STRING(1024),
                         ComposerInfo BYTES(MAX)
                     ) PRIMARY KEY (SingerId)",
            @"CREATE TABLE Albums (
                         SingerId INT64 NOT NULL,
                         AlbumId INT64 NOT NULL,
                         AlbumTitle STRING(MAX)
                     ) PRIMARY KEY (SingerId, AlbumId),
                     INTERLEAVE IN PARENT Singers ON DELETE CASCADE",
            @$"ALTER DATABASE `{databaseId}` SET OPTIONS
                   (version_retention_period = '{versionRetentionPeriod}')"
        );
        await createDbCommand.ExecuteNonQueryAsync();
    }
}
// [END spanner_create_database_with_version_retention_period]
