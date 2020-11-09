// Copyright 2020 Google Inc.
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

// [START spanner_dml_partitioned_update]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class UpdateUsingPartitionedDmlCoreAsyncSample
{
    public async Task<long> UpdateUsingPartitionedDmlCoreAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateDmlCommand("UPDATE Albums SET MarketingBudget = 100000 WHERE SingerId > 1");
        long rowCount = await cmd.ExecutePartitionedUpdateAsync();

        Console.WriteLine($"{rowCount} row(s) updated...");
        return rowCount;
    }
}
// [END spanner_dml_partitioned_update]
