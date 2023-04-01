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

// [START spanner_postgresql_add_column]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class AddColumnAsyncPostgresSample
{
    public async Task AddColumnAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        // PostgreSQL database with Singers table already exists.
        // Alter the Singers table.
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        string alterStatement = "ALTER TABLE Singers ADD COLUMN Age INTEGER";

        using var connection = new SpannerConnection(connectionString);
        using var updateCmd = connection.CreateDdlCommand(alterStatement);
        await updateCmd.ExecuteNonQueryAsync();
        Console.WriteLine("Added the Age column in Singers table.");
    }
}
// [END spanner_postgresql_add_column]
