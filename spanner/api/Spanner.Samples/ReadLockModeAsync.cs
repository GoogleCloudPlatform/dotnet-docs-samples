// Copyright 2025 Google Inc.
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

// [START spanner_read_lock_mode]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ReadLockModeAsyncSample
{
    public async Task ReadLockModeAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Create transaction options with ReadLockMode.Pessimistic.
        // This ensures that valid locks are acquired for read operations,
        // avoiding the need for a separate "dummy" update to lock the rows.
        var transactionOptions = SpannerTransactionCreationOptions.ReadWrite
            .WithReadLockMode(ReadLockMode.Pessimistic);

        using var transaction = await connection.BeginTransactionAsync(transactionOptions);
        // Read data. The transaction now holds locks on the read rows.
        var cmd = connection.CreateSelectCommand("SELECT * FROM Albums");
        cmd.Transaction = transaction;
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"SingerId: {reader.GetFieldValue<long>("SingerId")}, AlbumId: {reader.GetFieldValue<long>("AlbumId")}");
            }
        }

        // If we wanted to update the rows we just read, we could do so here
        // without worrying about race conditions because we have a lock.
        
        await transaction.CommitAsync();
    }
}
// [END spanner_read_lock_mode]
