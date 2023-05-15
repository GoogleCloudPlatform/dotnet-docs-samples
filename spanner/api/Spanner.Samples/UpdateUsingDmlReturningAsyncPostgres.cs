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

// [START spanner_postgresql_update_dml_returning]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UpdateUsingDmlReturningAsyncPostgresSample
{
    public async Task<List<long>> UpdateUsingDmlReturningAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Update MarketingBudget column for records satisfying
        // a particular condition and return the modified
        // MarketingBudget column of the updated records using
        // 'RETURNING MarketingBudget'.
        // It is also possible to return all columns of all the
        // updated records by using 'RETURNING *'.
        using var cmd = connection.CreateDmlCommand("UPDATE Albums SET MarketingBudget = MarketingBudget * 2 WHERE SingerId = 14 and AlbumId = 20 RETURNING MarketingBudget");

        var reader = await cmd.ExecuteReaderAsync();
        var updatedMarketingBudgets = new List<long>();
        while (await reader.ReadAsync())
        {
            updatedMarketingBudgets.Add(reader.GetFieldValue<long>("marketingbudget"));
        }

        Console.WriteLine($"{updatedMarketingBudgets.Count} row(s) updated...");
        return updatedMarketingBudgets;
    }
}
// [END spanner_postgresql_update_dml_returning]
