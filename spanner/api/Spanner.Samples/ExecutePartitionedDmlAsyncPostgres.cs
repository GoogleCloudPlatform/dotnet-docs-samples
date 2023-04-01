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

// [START spanner_postgresql_partitioned_dml]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class ExecutePartitionedDmlAsyncPostgresSample
{
    public async Task<long> ExecutePartitionedDmlAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // See https://cloud.google.com/spanner/docs/dml-partitioned for more information.
        using var cmd = connection.CreateDmlCommand("DELETE FROM Singers WHERE SingerId > 15");

        // The returned count is a lower bound of the number of records that was deleted.
        long rowCount = await cmd.ExecutePartitionedUpdateAsync();

        Console.WriteLine($"At least {rowCount} row(s) deleted...");
        return rowCount;
    }
}
// [END spanner_postgresql_partitioned_dml]
