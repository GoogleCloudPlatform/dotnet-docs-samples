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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class BatchReadRecordsAsyncSample
{
    private int _rowsRead;
    private int _partitionCount;
    public async Task<(int RowsRead, int Partitions)> BatchReadRecordsAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync(SpannerTransactionCreationOptions.ReadOnly.WithIsDetached(true), cancellationToken: default);
        transaction.DisposeBehavior = DisposeBehavior.CloseResources;
        using var cmd = connection.CreateSelectCommand("SELECT SingerId, FirstName, LastName FROM Singers");
        cmd.Transaction = transaction;

        // A CommandPartition object is serializable and can be used from a different process.
        // If data boost is enabled, partitioned read and query requests will be executed
        // using Spanner independent compute resources.
        var partitions = await cmd.GetReaderPartitionsAsync(PartitionOptions.Default.WithDataBoostEnabled(true));

        var transactionId = transaction.TransactionId;
        await Task.WhenAll(partitions.Select(x => DistributedReadWorkerAsync(x, transactionId)));
        Console.WriteLine($"Done reading!  Total rows read: {_rowsRead:N0} with {_partitionCount} partition(s)");
        return (RowsRead: _rowsRead, Partitions: _partitionCount);
    }

    private async Task DistributedReadWorkerAsync(CommandPartition readPartition, TransactionId id)
    {
        var localId = Interlocked.Increment(ref _partitionCount);
        using var connection = new SpannerConnection(id.ConnectionString);
        using var transaction = await connection.BeginTransactionAsync(SpannerTransactionCreationOptions.FromReadOnlyTransactionId(id), cancellationToken: default);
        using var cmd = connection.CreateCommandWithPartition(readPartition, transaction);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Interlocked.Increment(ref _rowsRead);
            Console.WriteLine($"Partition ({localId}) "
                + $"{reader.GetFieldValue<int>("SingerId")}"
                + $" {reader.GetFieldValue<string>("FirstName")}"
                + $" {reader.GetFieldValue<string>("LastName")}");
        }
        Console.WriteLine($"Done with single reader {localId}.");
    }
}
// [END spanner_batch_client]
