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

// [START spanner_read_write_transaction]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;
using System.Transactions;

public class ReadWriteWithTransactionAsyncSample
{
    public async Task<int> ReadWriteWithTransactionAsync(string projectId, string instanceId, string databaseId)
    {
        // This sample transfers 200,000 from the MarketingBudget
        // field of the second Album to the first Album. Make sure to run
        // the Add Column and Write Data To New Column samples first,
        // in that order.

        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        decimal transferAmount = 200000;
        decimal secondBudget = 0;
        decimal firstBudget = 0;

        using var connection = new SpannerConnection(connectionString);
        using var cmdLookup1 = connection.CreateSelectCommand("SELECT * FROM Albums WHERE SingerId = 2 AND AlbumId = 2");

        using (var reader = await cmdLookup1.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                // Read the second album's budget.
                secondBudget = reader.GetFieldValue<decimal>("MarketingBudget");
                // Confirm second Album's budget is sufficient and
                // if not raise an exception. Raising an exception
                // will automatically roll back the transaction.
                if (secondBudget < transferAmount)
                {
                    throw new Exception($"The second album's budget {secondBudget} is less than the amount to transfer.");
                }
            }
        }

        // Read the first album's budget.
        using var cmdLookup2 = connection.CreateSelectCommand("SELECT * FROM Albums WHERE SingerId = 1 and AlbumId = 1");
        using (var reader = await cmdLookup2.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                firstBudget = reader.GetFieldValue<decimal>("MarketingBudget");
            }
        }

        // Specify update command parameters.
        using var cmdUpdate = connection.CreateUpdateCommand("Albums", new SpannerParameterCollection
        {
            { "SingerId", SpannerDbType.Int64 },
            { "AlbumId", SpannerDbType.Int64 },
            { "MarketingBudget", SpannerDbType.Int64 },
        });

        // Update second album to remove the transfer amount.
        secondBudget -= transferAmount;
        cmdUpdate.Parameters["SingerId"].Value = 2;
        cmdUpdate.Parameters["AlbumId"].Value = 2;
        cmdUpdate.Parameters["MarketingBudget"].Value = secondBudget;
        var rowCount = await cmdUpdate.ExecuteNonQueryAsync();

        // Update first album to add the transfer amount.
        firstBudget += transferAmount;
        cmdUpdate.Parameters["SingerId"].Value = 1;
        cmdUpdate.Parameters["AlbumId"].Value = 1;
        cmdUpdate.Parameters["MarketingBudget"].Value = firstBudget;
        rowCount += await cmdUpdate.ExecuteNonQueryAsync();
        scope.Complete();
        Console.WriteLine("Transaction complete.");
        return rowCount;
    }
}
// [END spanner_read_write_transaction]
