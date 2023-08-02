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

// [START spanner_postgresql_create_sequence]

using Google.Cloud.Spanner.Common.V1;
using System.Threading.Tasks;
using System;
using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Data;
using System.Collections.Generic;

public class CreateSequencePostgresqlSample
{
    public async Task<List<long>> CreateSequencePostgresqlSampleAsync(string projectId, string instanceId, string databaseId)
    {
        DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

        DatabaseName databaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);
        string[] statements =
        {
            "CREATE SEQUENCE Seq BIT_REVERSED_POSITIVE ;",
            "CREATE TABLE Customers (CustomerId BIGINT DEFAULT nextval('Seq'), CustomerName character varying(1024), PRIMARY KEY (CustomerId))"
        };
        var operation = await databaseAdminClient.UpdateDatabaseDdlAsync(databaseName, statements);

        var completedResponse = await operation.PollUntilCompletedAsync();
        if (completedResponse.IsFaulted)
        {
            throw completedResponse.Exception;
        }

        Console.WriteLine("Created Seq sequence and Customers table, where the key column CustomerId uses the sequence as a default value");

        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        using var cmd = connection.CreateDmlCommand(
            @"INSERT INTO Customers (CustomerName) VALUES ('Alice'), ('David'), ('Marc') RETURNING CustomerId");

        var reader = await cmd.ExecuteReaderAsync();
        var customerIds = new List<long>();
        while (await reader.ReadAsync())
        {
            var customerId = reader.GetFieldValue<long>("customerid");
            Console.WriteLine($"Inserted customer record with CustomerId: {customerId}");
            customerIds.Add(customerId);
        }
        Console.WriteLine($"Number of customer records inserted is: {customerIds.Count}");
        return customerIds;
    }
}
// [END spanner_postgresql_create_sequence]
