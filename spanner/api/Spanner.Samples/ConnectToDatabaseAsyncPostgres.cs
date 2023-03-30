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

// [START spanner_connect_postgresql_database]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class ConnectToDatabaseAsyncPostgresSample
{
    public async Task<string> ConnectToDatabaseAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateSelectCommand("SELECT 'Hello from Spanner PostgreSQL!'");

        var result = await command.ExecuteScalarAsync<string>();
        Console.WriteLine(result);
        return result;
    }
}
// [END spanner_connect_postgresql_database]
