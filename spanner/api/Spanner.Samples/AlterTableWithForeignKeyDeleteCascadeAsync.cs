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

// [START spanner_alter_table_with_foreign_key_delete_cascade]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class AlterTableWithForeignKeyDeleteCascadeAsyncSample
{
    public async Task AlterTableWithForeignKeyDeleteCascadeAsync(string projectId, string instanceId, string databaseId)
    {
        var connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        var alterStatement =
            @"ALTER TABLE ShoppingCarts
                ADD CONSTRAINT FKShoppingCartsCustomerName
                FOREIGN KEY (CustomerName)
                REFERENCES Customers(CustomerName)
                ON DELETE CASCADE";

        using var connection = new SpannerConnection(connectionString);
        var alterCmd = connection.CreateDdlCommand(alterStatement);
        await alterCmd.ExecuteNonQueryAsync();
        Console.WriteLine(@$"Altered ShoppingCarts table with FKShoppingCartsCustomerName
            foreign key constraint on database {databaseId} on instance {instanceId}");
    }
}
// [END spanner_alter_table_with_foreign_key_delete_cascade]
