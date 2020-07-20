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

// [START spanner_batch_client]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class BatchReadRecordsAsyncSample
{
    int s_rowsRead;
    int s_partitionId;
    public async Task<List<int>> BatchReadRecordsAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var transaction = await connection.BeginReadOnlyTransactionAsync())
            using (var cmd = connection.CreateSelectCommand("SELECT SingerId, FirstName, LastName FROM Singers"))
            {
                transaction.DisposeBehavior = DisposeBehavior.CloseResources;
                cmd.Transaction = transaction;
                var partitions = await cmd.GetReaderPartitionsAsync();
                var transactionId = transaction.TransactionId;
                await Task.WhenAll(partitions.Select(x => DistributedReadWorkerAsync(x, transactionId)))
                    .ConfigureAwait(false);
            }
            Console.WriteLine($"Done reading!  Total rows read: {s_rowsRead:N0} with {s_partitionId} partition(s)");
            return new List<int> { s_rowsRead, s_partitionId };
        }
    }

    private async Task DistributedReadWorkerAsync(CommandPartition readPartition, TransactionId id)
    {
        var localId = Interlocked.Increment(ref s_partitionId);
        using (var connection = new SpannerConnection(id.ConnectionString))
        using (var transaction = connection.BeginReadOnlyTransaction(id))
        {
            using (var cmd = connection.CreateCommandWithPartition(readPartition, transaction))
            {
                using (var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync())
                    {
                        Interlocked.Increment(ref s_rowsRead);
                        Console.WriteLine($"Partition ({localId}) "
                            + $"{reader.GetFieldValue<string>("SingerId")}"
                            + $" {reader.GetFieldValue<string>("FirstName")}"
                            + $" {reader.GetFieldValue<string>("LastName")}");
                    }
                }
            }
            Console.WriteLine($"Done with single reader {localId}.");
        }
    }
}
// [END spanner_batch_client]
