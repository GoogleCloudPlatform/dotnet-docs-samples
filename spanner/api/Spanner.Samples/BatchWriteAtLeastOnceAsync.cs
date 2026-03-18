// Copyright 2026 Google LLC
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

// [START spanner_batch_write_at_least_once]

using Google.Cloud.Spanner.Data;
using Google.Rpc;
using Google.Cloud.Spanner.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BatchWriteAtLeastOnceAsyncSample
{
    public async Task BatchWriteAtLeastOnceAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // 1. Create a SpannerBatchWriteCommand.
        var cmd = connection.CreateBatchWriteCommand();

        // 2. Create and add mutation groups.
        // Mutation group 1: Insert or update a singer (16).
        var cmdSinger1 = connection.CreateInsertOrUpdateCommand("Singers", new SpannerParameterCollection
        {
            { "SingerId", SpannerDbType.Int64, 16 },
            { "FirstName", SpannerDbType.String, "Scarlet" },
            { "LastName", SpannerDbType.String, "Terry" }
        });
        cmd.Add(cmdSinger1);

        // Mutation group 2: Insert or update multiple singers and albums together.
        var cmdSinger2 = connection.CreateInsertOrUpdateCommand("Singers", new SpannerParameterCollection
        {
            { "SingerId", SpannerDbType.Int64, 17 },
            { "FirstName", SpannerDbType.String, "Marc" },
            { "LastName", SpannerDbType.String, "Smith" }
        });

        var cmdSinger3 = connection.CreateInsertOrUpdateCommand("Singers", new SpannerParameterCollection
        {
            { "SingerId", SpannerDbType.Int64, 18 },
            { "FirstName", SpannerDbType.String, "Catalina" },
            { "LastName", SpannerDbType.String, "Smith" }
        });

        var cmdAlbum1 = connection.CreateInsertOrUpdateCommand("Albums", new SpannerParameterCollection
        {
            { "SingerId", SpannerDbType.Int64, 17 },
            { "AlbumId", SpannerDbType.Int64, 1 },
            { "AlbumTitle", SpannerDbType.String, "Total Junk" }
        });

        var cmdAlbum2 = connection.CreateInsertOrUpdateCommand("Albums", new SpannerParameterCollection
        {
            { "SingerId", SpannerDbType.Int64, 18 },
            { "AlbumId", SpannerDbType.Int64, 2 },
            { "AlbumTitle", SpannerDbType.String, "Go, Go, Go" }
        });

        // Add multiple commands to the same group.
        cmd.Add(cmdSinger2, cmdSinger3, cmdAlbum1, cmdAlbum2);

        // 3. Execute the batch write request.
        try
        {
            // ExecuteNonQueryAsync returns an IAsyncEnumerable of BatchWriteResponse.
            await foreach (var response in cmd.ExecuteNonQueryAsync())
            {
                if (response.Status?.Code == (int)Code.Ok)
                {
                    Console.WriteLine($"Mutation group indexes {string.Join(", ", response.Indexes)} have been applied with commit timestamp {response.CommitTimestamp}");
                }
                else
                {
                    Console.WriteLine($"Mutation group indexes {string.Join(", ", response.Indexes)} could not be applied with error code {response.Status?.Code}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing batch write: {ex.Message}");
            throw;
        }
    }
}
// [END spanner_batch_write_at_least_once]
