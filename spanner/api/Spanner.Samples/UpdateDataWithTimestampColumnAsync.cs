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

// [START spanner_update_data_with_timestamp_column]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class UpdateDataWithTimestampColumnAsyncSample
{
    public async Task<int> UpdateDataWithTimestampColumnAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);

        var rowCount = 0;
        using var updateCmd1 = connection.CreateUpdateCommand("Albums", new SpannerParameterCollection
        {
            { "SingerId", SpannerDbType.Int64, 1 },
            { "AlbumId", SpannerDbType.Int64, 1 },
            { "MarketingBudget", SpannerDbType.Int64, 1000000 },
            { "LastUpdateTime", SpannerDbType.Timestamp, SpannerParameter.CommitTimestamp },
        });
        rowCount += await updateCmd1.ExecuteNonQueryAsync();

        using var updateCmd2 = connection.CreateUpdateCommand("Albums", new SpannerParameterCollection
        {
            { "SingerId", SpannerDbType.Int64, 2 },
            { "AlbumId", SpannerDbType.Int64, 2 },
            { "MarketingBudget", SpannerDbType.Int64, 750000 },
            { "LastUpdateTime", SpannerDbType.Timestamp, SpannerParameter.CommitTimestamp },
        });
        rowCount += await updateCmd2.ExecuteNonQueryAsync();

        Console.WriteLine("Updated data.");
        return rowCount;
    }
}
// [END spanner_update_data_with_timestamp_column]
