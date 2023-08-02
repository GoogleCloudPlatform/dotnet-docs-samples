// Copyright 2023 Google Inc.
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

// [START spanner_alter_sequence]

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AlterSequenceSample
{
    public async Task<List<long>> AlterSequenceSampleAsync(string projectId, string instanceId, string databaseId)
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        DatabaseName databaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);
        string[] statements =
        {
            "ALTER SEQUENCE Seq SET OPTIONS (skip_range_min = 1000, skip_range_max = 5000000)"
        };
        var operation = await databaseAdminClient.UpdateDatabaseDdlAsync(databaseName, statements);

        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            throw completedResponse.Exception;
        }
        Console.WriteLine("Altered Seq sequence to skip an inclusive range between 1000 and 5000000");

        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateDmlCommand(
            @"INSERT INTO Customers (CustomerName) VALUES ('Alice'), ('David'), ('Marc') THEN RETURN CustomerId");

        var reader = await cmd.ExecuteReaderAsync();
        var customerIds = new List<long>();
        while (await reader.ReadAsync())
        {
            long customerId = reader.GetFieldValue<long>("CustomerId");
            Console.WriteLine($"Inserted customer record with CustomerId: {customerId}");
            customerIds.Add(customerId);
        }
        Console.WriteLine($"Number of customer records inserted is: {customerIds.Count}");

        return customerIds;
    }
}
// [END spanner_alter_sequence]
