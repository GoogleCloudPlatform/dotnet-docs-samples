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

// [START spanner_ack_queue_message_with_mutation_api]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class AckQueueMessageWithMutationAsyncSample
{
    public async Task AckQueueMessageWithMutationAsync(string projectId, string instanceId, string databaseId, long messageId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateAckCommand("MyQueue", new SpannerParameterCollection
        {
            { "Id", SpannerDbType.Int64, messageId }
        });
        // By default, acknowledging a message that does not exist fails.
        // Set cmd.AckOptions = new AckOptions { IgnoreNotFound = true } to allow it.
        await cmd.ExecuteNonQueryAsync();

        Console.WriteLine($"Acknowledged message {messageId} in queue MyQueue");
    }
}
// [END spanner_ack_queue_message_with_mutation_api]
