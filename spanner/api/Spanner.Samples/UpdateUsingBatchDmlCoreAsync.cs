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

// [START spanner_dml_batch_update]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UpdateUsingBatchDmlCoreAsyncSample
{
    public async Task<int> UpdateUsingBatchDmlCoreAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        SpannerBatchCommand cmd = connection.CreateBatchDmlCommand();

        cmd.Add("INSERT INTO Albums (SingerId, AlbumId, AlbumTitle, MarketingBudget) VALUES (1, 3, 'Test Album Title', 10000)");

        cmd.Add("UPDATE Albums SET MarketingBudget = MarketingBudget * 2 WHERE SingerId = 1 and AlbumId = 3");

        IEnumerable<long> affectedRows = await cmd.ExecuteNonQueryAsync();

        Console.WriteLine($"Executed {affectedRows.Count()} " + "SQL statements using Batch DML.");
        return affectedRows.Count();
    }
}
// [END spanner_dml_batch_update]
