// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START spanner_send_to_queue_with_sql_api]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class SendToQueueWithSqlAsyncSample
{
    public async Task<int> SendToQueueWithSqlAsync(string projectId, string instanceId, string databaseId, long messageId, string payload)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateDmlCommand(
            "INSERT INTO MyQueue (Id, Payload) VALUES (@Id, @Payload)",
            new SpannerParameterCollection
            {
                { "Id", SpannerDbType.Int64, messageId },
                { "Payload", SpannerDbType.String, payload }
            });
        int rowCount = await cmd.ExecuteNonQueryAsync();

        Console.WriteLine($"Sent message {messageId} to queue MyQueue");
        return rowCount;
    }
}
// [END spanner_send_to_queue_with_sql_api]
