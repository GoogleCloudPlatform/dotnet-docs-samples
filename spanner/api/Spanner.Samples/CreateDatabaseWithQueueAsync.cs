// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START spanner_create_database_with_queue]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class CreateDatabaseWithQueueAsyncSample
{
    public async Task CreateDatabaseWithQueueAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}";

        using var connection = new SpannerConnection(connectionString);
        string createDatabase = $"CREATE DATABASE `{databaseId}`";
        string createQueue =
            @"CREATE QUEUE MyQueue (
                Id INT64 NOT NULL,
                Payload STRING(MAX) NOT NULL
            ) PRIMARY KEY (Id),
            OPTIONS (receive_mode = 'PULL')";

        using var createDbCommand = connection.CreateDdlCommand(createDatabase, createQueue);
        await createDbCommand.ExecuteNonQueryAsync();

        Console.WriteLine($"Created database {databaseId} with queue MyQueue");
    }
}
// [END spanner_create_database_with_queue]
