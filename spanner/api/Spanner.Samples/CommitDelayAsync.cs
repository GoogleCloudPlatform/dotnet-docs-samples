// Copyright 2024 Google Inc.
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

// [START spanner_set_max_commit_delay]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class CommitDelayAsyncSample
{
    public async Task<int> CommitDelayAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        return await connection.RunWithRetriableTransactionAsync(async transaction =>
        {
            transaction.MaxCommitDelay = TimeSpan.FromMilliseconds(100);

            using var insertSingerCmd = connection.CreateInsertCommand("Singers",
                new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.Int64, 1 },
                    { "FirstName", SpannerDbType.String, "Marc" },
                    { "LastName", SpannerDbType.String, "Richards" }
                });
            insertSingerCmd.Transaction = transaction;
            int rowsInserted = await insertSingerCmd.ExecuteNonQueryAsync();

            using var insertAlbumCmd = connection.CreateInsertCommand("Albums",
                new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.Int64, 1 },
                    { "AlbumId", SpannerDbType.Int64, 2 },
                    { "AlbumTitle", SpannerDbType.String, "Go, Go, Go" }
                });
            insertAlbumCmd.Transaction = transaction;
            rowsInserted += await insertAlbumCmd.ExecuteNonQueryAsync();

            return rowsInserted;
        });
    }
}
// [END spanner_set_max_commit_delay]
