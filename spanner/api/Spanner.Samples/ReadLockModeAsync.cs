// Copyright 2026 Google Inc.
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
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId};ReadLockMode=Optimistic";

        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Create transaction options with ReadLockMode.Pessimistic.
        var transactionOptions = SpannerTransactionCreationOptions.ReadWrite
            .WithReadLockMode(ReadLockMode.Pessimistic);

        using var transaction = await connection.BeginTransactionAsync(transactionOptions, null, CancellationToken.None);

        var cmd = connection.CreateSelectCommand("SELECT * FROM Albums");
        cmd.Transaction = transaction;
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"SingerId: {reader.GetFieldValue<long>("SingerId")}, AlbumId: {reader.GetFieldValue<long>("AlbumId")}");
            }
        }


        await transaction.CommitAsync();
    }
}
// [END spanner_read_lock_mode]
