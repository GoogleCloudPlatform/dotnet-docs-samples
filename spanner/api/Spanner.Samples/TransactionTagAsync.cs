// Copyright 2021 Google Inc.
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

// [START spanner_set_transaction_tag]

using Google.Cloud.Spanner.Data;
using System.Threading.Tasks;

public class TransactionTagAsyncSample
{
    public async Task<int> TransactionTagAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        return await connection.RunWithRetriableTransactionAsync(async transaction =>
        {
            // Sets the transaction tag to "app=concert,env=dev".
            // This transaction tag will be applied to all the individual operations inside
            // the transaction.
            transaction.Tag = "app=concert,env=dev";
            
            // Sets the request tag to "app=concert,env=dev,action=update".
            // This request tag will only be set on this request.
            var updateCommand =
                connection.CreateDmlCommand("UPDATE Venues SET Capacity = DIV(Capacity, 4) WHERE OutdoorVenue = false");
            updateCommand.Tag = "app=concert,env=dev,action=update";
            updateCommand.Transaction = transaction;
            int rowsModified = await updateCommand.ExecuteNonQueryAsync();

            var insertCommand = connection.CreateDmlCommand(
                @"INSERT INTO Venues (VenueId, VenueName, Capacity, OutdoorVenue, LastUpdateTime)
                    VALUES (@venueId, @venueName, @capacity, @outdoorVenue, PENDING_COMMIT_TIMESTAMP())",
                new SpannerParameterCollection
                {
                    {"venueId", SpannerDbType.Int64, 81},
                    {"venueName", SpannerDbType.String, "Venue 81"},
                    {"capacity", SpannerDbType.Int64, 1440},
                    {"outdoorVenue", SpannerDbType.Bool, true}
                }
            );
            // Sets the request tag to "app=concert,env=dev,action=insert".
            // This request tag will only be set on this request.
            insertCommand.Tag = "app=concert,env=dev,action=insert";
            insertCommand.Transaction = transaction;
            rowsModified += await insertCommand.ExecuteNonQueryAsync();
            return rowsModified;
        });
    }
}
// [END spanner_set_transaction_tag]
