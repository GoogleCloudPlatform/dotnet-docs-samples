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

// [START spanner_postgresql_dml_getting_started_update]

using Google.Cloud.Spanner.Data;
using Google.Cloud.Spanner.V1;
using System;
using System.Threading.Tasks;

public class UpdateUsingDmlAsyncPostgresSample
{
    public async Task<int> UpdateUsingDmlAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateDmlCommand("UPDATE Singers SET Rating = $1 WHERE SingerId = 11", 
            new SpannerParameterCollection 
            {
                { "p1", SpannerDbType.PgNumeric, PgNumeric.Parse("4.0") }
            });

        var rowCount = await cmd.ExecuteNonQueryAsync();

        Console.WriteLine($"{rowCount} row(s) updated...");
        return rowCount;
    }
}
// [END spanner_postgresql_dml_getting_started_update]
