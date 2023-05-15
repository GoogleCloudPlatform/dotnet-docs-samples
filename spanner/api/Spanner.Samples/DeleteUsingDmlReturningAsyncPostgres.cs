// Copyright 2023 Google Inc.
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

// [START spanner_postgresql_delete_dml_returning]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DeleteUsingDmlReturningAsyncPostgresSample
{
    public async Task<List<string>> DeleteUsingDmlReturningAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Delete records from SINGERS table satisfying a
        // particular condition and return the SingerId
        // and FullName column of the deleted records using
        // 'RETURNING SingerId, FullName'.
        // It is also possible to return all columns of all the
        // deleted records by using 'RETURNING *'.
        using var cmd = connection.CreateDmlCommand("DELETE FROM Singers WHERE FirstName = 'Lata' RETURNING SingerId, FullName");
        var reader = await cmd.ExecuteReaderAsync();
        var deletedSingerNames = new List<string>();
        while (await reader.ReadAsync())
        {
            deletedSingerNames.Add(reader.GetFieldValue<string>("fullname"));
        }

        Console.WriteLine($"{deletedSingerNames.Count} row(s) deleted...");
        return deletedSingerNames;
    }
}
// [END spanner_postgresql_delete_dml_returning]
