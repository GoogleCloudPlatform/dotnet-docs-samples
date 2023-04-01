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

// [START spanner_postgresql_functions]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class UseFunctionAsyncPostgresSample
{
    public async Task<DateTime> UseFunctionAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Use the PostgreSQL `to_timestamp` function to convert a number of seconds since epoch to a
        // timestamp. 1284352323 seconds = Monday, September 13, 2010 4:32:03 AM.
        var command = connection.CreateSelectCommand("SELECT to_timestamp(1284352323) AS t");

        var result = await command.ExecuteScalarAsync<DateTime>();
        Console.WriteLine(result);
        return result;
    }
}
// [END spanner_postgresql_functions]
