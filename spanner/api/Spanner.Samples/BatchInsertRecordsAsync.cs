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

// [START spanner_dml_batch_insert]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class BatchInsertRecordsAsyncSample
{
    public async Task BatchInsertRecordsAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        // Get current max SingerId, start at a minimum SingerId of 20.
        Int64 maxSingerId = 20;
        using (var connection = new SpannerConnection(connectionString))
        {
            // Execute a SQL statement to get current MAX() of SingerId.
            var cmd = connection.CreateSelectCommand(@"SELECT MAX(SingerId) as SingerId FROM Singers");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    long parsedValue;
                    bool result = Int64.TryParse(reader.GetFieldValue<string>("SingerId"), out parsedValue);
                    if (result)
                    {
                        maxSingerId += parsedValue;
                    }
                }
            }
        }

        // Batch insert 249,900 singer records into the Singers table.
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            for (int i = 0; i < 100; i++)
            {
                // For details on transaction isolation, see the "Isolation" section in:
                // https://cloud.google.com/spanner/docs/transactions#read-write_transactions
                using (var tx = await connection.BeginTransactionAsync())
                using (var cmd = connection.CreateInsertCommand("Singers", new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.String },
                    { "FirstName", SpannerDbType.String },
                    { "LastName", SpannerDbType.String }
                }))
                {
                    cmd.Transaction = tx;
                    for (var x = 1; x < 2500; x++)
                    {
                        maxSingerId++;
                        string nameSuffix = Guid.NewGuid().ToString().Substring(0, 8);
                        cmd.Parameters["SingerId"].Value = maxSingerId;
                        cmd.Parameters["FirstName"].Value = $"FirstName-{nameSuffix}";
                        cmd.Parameters["LastName"].Value = $"LastName-{nameSuffix}";
                        cmd.ExecuteNonQuery();
                    }
                    await tx.CommitAsync();
                }
            }
        }
        Console.WriteLine("Done inserting sample records...");
    }
}
// [END spanner_dml_batch_insert]
