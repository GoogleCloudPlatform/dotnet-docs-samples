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

// [START spanner_postgresql_batch_dml]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class InsertUsingBatchDmlAsyncPostgresSample
{
    public async Task<int> InsertUsingBatchDmlAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        SpannerBatchCommand batchCommand = connection.CreateBatchDmlCommand();

        // Simple PostgreSQL DML statement.
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (1, 'Alice', 'Henderson')");
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (2, 'Bruce', 'Allison')");

        // Example of parameterized PostgreSQL DML statement. Named parameters are not supported in current Spanner PostgreSQL implementation.
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES ($1, $2, $3)",
            new SpannerParameterCollection
            {
                    { "p1", SpannerDbType.Int64, 3L },
                    { "p2", SpannerDbType.String, "Olivia" },
                    { "p3", SpannerDbType.String, "Garcia" }
            });

        IEnumerable<long> affectedRows = await batchCommand.ExecuteNonQueryAsync();

        int count = affectedRows.Count();
        Console.WriteLine($"Executed {count} PostgreSQL statements using Batch DML.");
        return count;
    }
}
// [END spanner_postgresql_batch_dml]
