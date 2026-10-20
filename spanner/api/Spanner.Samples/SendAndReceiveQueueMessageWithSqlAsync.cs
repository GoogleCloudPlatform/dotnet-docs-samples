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

// [START spanner_send_and_receive_queue_message_with_sql_api]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class SendAndReceiveQueueMessageWithSqlAsyncSample
{
    public async Task<string> SendAndReceiveQueueMessageWithSqlAsync(string projectId, string instanceId, string databaseId, long messageId, string payload)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Send a message to the queue.
        using var sendCmd = connection.CreateDmlCommand(
            "INSERT INTO MyQueue (Id, Payload) VALUES (@Id, @Payload)",
            new SpannerParameterCollection
            {
                { "Id", SpannerDbType.Int64, messageId },
                { "Payload", SpannerDbType.String, payload }
            });
        await sendCmd.ExecuteNonQueryAsync();
        Console.WriteLine($"Sent message {messageId} to queue MyQueue");

        // Receive messages from the queue using the RECEIVE_<queue name> table-valued function.
        // The receive call keeps the stream open for up to max_duration while waiting for messages.
        // Each read waits for the next message and must complete within the command timeout
        // (60 seconds by default), so the command timeout must be longer than the time between
        // received messages.
        using var receiveCmd = connection.CreateSelectCommand(
            "SELECT Id, Payload FROM RECEIVE_MyQueue(max_duration => '10s')");
        receiveCmd.CommandTimeout = 30;

        using var reader = await receiveCmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            Console.WriteLine("No message received from queue MyQueue");
            return null;
        }

        long receivedId = reader.GetFieldValue<long>("Id");
        string receivedPayload = reader.GetFieldValue<string>("Payload");
        Console.WriteLine($"Received message {receivedId}: {receivedPayload}");

        return receivedPayload;
    }
}
// [END spanner_send_and_receive_queue_message_with_sql_api]
