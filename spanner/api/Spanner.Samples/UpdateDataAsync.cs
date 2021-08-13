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

// [START spanner_update_data]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class UpdateDataAsyncSample
{
    public async Task<int> UpdateDataAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);

        var rowCount = 0;
        SpannerCommand cmd = connection.CreateDmlCommand(
            "UPDATE Albums SET MarketingBudget = @MarketingBudget "
            + "WHERE SingerId = 1 and AlbumId = 1");
        cmd.Parameters.Add("MarketingBudget", SpannerDbType.Int64, 100000);
        rowCount += await cmd.ExecuteNonQueryAsync();

        cmd = connection.CreateDmlCommand(
            "UPDATE Albums SET MarketingBudget = @MarketingBudget "
            + "WHERE SingerId = 2 and AlbumId = 2");
        cmd.Parameters.Add("MarketingBudget", SpannerDbType.Int64, 500000);
        rowCount += await cmd.ExecuteNonQueryAsync();

        Console.WriteLine("Data Updated.");
        return rowCount;
    }
}
// [END spanner_update_data]
