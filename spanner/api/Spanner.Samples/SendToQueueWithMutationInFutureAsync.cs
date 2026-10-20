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

// [START spanner_send_to_queue_with_mutation_api_in_future]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class SendToQueueWithMutationInFutureAsyncSample
{
    public async Task SendToQueueWithMutationInFutureAsync(string projectId, string instanceId, string databaseId, long messageId, string payload, DateTimeOffset deliverTime)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateSendCommand("MyQueue", new SpannerParameterCollection
        {
            { "Id", SpannerDbType.Int64, messageId },
            { "Payload", SpannerDbType.String, payload }
        });
        // The message will not be delivered to receivers until the specified time.
        cmd.SendOptions = new SendOptions { DeliverTime = deliverTime };
        await cmd.ExecuteNonQueryAsync();

        Console.WriteLine($"Sent message {messageId} to queue MyQueue for delivery at {deliverTime:O}");
    }
}
// [END spanner_send_to_queue_with_mutation_api_in_future]
