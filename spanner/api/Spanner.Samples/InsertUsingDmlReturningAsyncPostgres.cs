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

// [START spanner_postgresql_insert_dml_returning]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InsertUsingDmlReturningAsyncPostgresSample
{
    public async Task<List<string>> InsertUsingDmlReturningAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Insert records into SINGERS table and return the
        // generated column FullName of the inserted records
        // using 'RETURNING FullName'.
        // It is also possible to return all columns of all the
        // inserted records by using 'RETURNING *'.
        using var cmd = connection.CreateDmlCommand(
            @"INSERT INTO Singers(SingerId, FirstName, LastName) VALUES
            (6, 'Melissa', 'Garcia'), 
            (7, 'Russell', 'Morales'), 
            (8, 'Jacqueline', 'Long'), 
            (9, 'Dylan', 'Shaw') RETURNING FullName");

        var reader = await cmd.ExecuteReaderAsync();
        var insertedSingerNames = new List<string>();
        while (await reader.ReadAsync())
        {
            insertedSingerNames.Add(reader.GetFieldValue<string>("fullname"));
        }

        Console.WriteLine($"{insertedSingerNames.Count} row(s) inserted...");
        return insertedSingerNames;
    }
}
// [END spanner_postgresql_insert_dml_returning]
