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
    public async Task ReadWriteWithTransactionAsync(string projectId, string instanceId, string databaseId)
    {
        // This sample transfers 200,000 from the MarketingBudget
        // field of the second Album to the first Album. Make sure to run
        // the addColumn and writeDataToNewColumn samples first,
        // in that order.

        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            decimal transferAmount = 200000;
            decimal secondBudget = 0;
            decimal firstBudget = 0;

            // Create connection to Cloud Spanner.
            using (var connection = new SpannerConnection(connectionString))
            {
                // Create statement to select the second album's data.
                var cmdLookup = connection.CreateSelectCommand("SELECT * FROM Albums WHERE SingerId = 2 AND AlbumId = 2");
                // Execute the select query.
                using (var reader = await cmdLookup.ExecuteReaderAsync())
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
                cmdLookup = connection.CreateSelectCommand("SELECT * FROM Albums WHERE SingerId = 1 and AlbumId = 1");
                using (var reader = await cmdLookup.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        firstBudget = reader.GetFieldValue<decimal>("MarketingBudget");
                    }
                }

                // Specify update command parameters.
                var cmd = connection.CreateUpdateCommand("Albums", new SpannerParameterCollection {
                    {"SingerId", SpannerDbType.Int64},
                    {"AlbumId", SpannerDbType.Int64},
                    {"MarketingBudget", SpannerDbType.Int64}
                });

                // Update second album to remove the transfer amount.
                secondBudget -= transferAmount;
                cmd.Parameters["SingerId"].Value = 2;
                cmd.Parameters["AlbumId"].Value = 2;
                cmd.Parameters["MarketingBudget"].Value = secondBudget;
                await cmd.ExecuteNonQueryAsync();

                // Update first album to add the transfer amount.
                firstBudget += transferAmount;
                cmd.Parameters["SingerId"].Value = 1;
                cmd.Parameters["AlbumId"].Value = 1;
                cmd.Parameters["MarketingBudget"].Value = firstBudget;
                await cmd.ExecuteNonQueryAsync();
                scope.Complete();
                Console.WriteLine("Transaction complete.");
            }
        }
    }
}
// [END spanner_read_write_transaction]
