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

// [START spanner_read_only_transaction]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;
using System.Transactions;

public class QueryDataWithTransactionAsyncSample
{
    public async Task QueryDataWithTransactionAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Gets a transaction object that captures the database state
        // at a specific point in time.
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                // Open the connection, making the implicitly created
                // transaction read only when it connects to the outer
                // transaction scope.
                await connection.OpenAsReadOnlyAsync().ConfigureAwait(false);
                var cmd = connection.CreateSelectCommand("SELECT SingerId, AlbumId, AlbumTitle FROM Albums");

                // Read #1.
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("SingerId : " + reader.GetFieldValue<string>("SingerId")
                            + " AlbumId : " + reader.GetFieldValue<string>("AlbumId")
                            + " AlbumTitle : " + reader.GetFieldValue<string>("AlbumTitle"));
                    }
                }
                // Read #2. Even if changes occur in-between the reads,
                // the transaction ensures that Read #1 and Read #2
                // return the same data.
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Console.WriteLine("SingerId : " + reader.GetFieldValue<string>("SingerId")
                            + " AlbumId : " + reader.GetFieldValue<string>("AlbumId")
                            + " AlbumTitle : " + reader.GetFieldValue<string>("AlbumTitle"));
                    }
                }
            }
            scope.Complete();
            Console.WriteLine("Transaction complete.");
        }
    }
}
// [END spanner_read_only_transaction]
