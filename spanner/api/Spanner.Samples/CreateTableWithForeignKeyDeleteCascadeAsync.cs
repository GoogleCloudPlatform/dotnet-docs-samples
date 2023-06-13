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

// [START spanner_create_table_with_foreign_key_delete_cascade]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class CreateTableWithForeignKeyDeleteCascadeAsyncSample
{
    public async Task CreateTableWithForeignKeyDeleteCascadeAsync(string projectId, string instanceId, string databaseId)
    {
        var connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);

        var createCustomerTableStatement =
            @"CREATE TABLE Customers(
            CustomerId INT64 NOT NULL, 
            CustomerName STRING(62) NOT NULL
            ) PRIMARY KEY (CustomerId)";

        var createShoppingCartTableStatement =
            @"CREATE TABLE ShoppingCarts (
            CartId INT64 NOT NULL, 
            CustomerId INT64 NOT NULL, 
            CustomerName STRING(62) NOT NULL, 
            CONSTRAINT FKShoppingCartsCustomerId FOREIGN KEY (CustomerId) 
            REFERENCES Customers (CustomerId) ON DELETE CASCADE
            ) PRIMARY KEY (CartId)";

        var cmd = connection.CreateDdlCommand(createCustomerTableStatement, createShoppingCartTableStatement);
        await cmd.ExecuteNonQueryAsync();

        Console.WriteLine(@$"Created Customers and ShoppingCarts table with FKShoppingCartsCustomerId
            foreign key constraint on database {databaseId} on instance {instanceId}");
    }
}
// [END spanner_create_table_with_foreign_key_delete_cascade]
