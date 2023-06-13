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

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class AlterTableWithForeignKeyDeleteCascadeAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public AlterTableWithForeignKeyDeleteCascadeAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task AlterTableWithForeignKeyDeleteCascadeAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var connectionString = $"Data Source=projects/{_spannerFixture.ProjectId}/instances/{_spannerFixture.InstanceId}/databases/{databaseId}";
            using var connection = new SpannerConnection(connectionString);
            await CreateTables(connection);

            var sample = new AlterTableWithForeignKeyDeleteCascadeAsyncSample();
            await sample.AlterTableWithForeignKeyDeleteCascadeAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            await AssertAlteredDeleteRuleConstraintsAsync(connection);
        });
    }

    private async Task CreateTables(SpannerConnection connection)
    {
        var createCustomerTableStatement =
        @"CREATE TABLE Customers(
            CustomerId INT64 NOT NULL, 
            CustomerName STRING(62) NOT NULL
            ) PRIMARY KEY (CustomerId)";

        var createShoppingCartTableStatement =
            @"CREATE TABLE ShoppingCarts (
            CartId INT64 NOT NULL, 
            CustomerId INT64 NOT NULL, 
            CustomerName STRING(62) NOT NULL
            ) PRIMARY KEY (CartId)";

        var cmd = connection.CreateDdlCommand(createCustomerTableStatement, createShoppingCartTableStatement);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task AssertAlteredDeleteRuleConstraintsAsync(SpannerConnection connection)
    {
        var command = connection.CreateSelectCommand(
            @"SELECT Count(*) FROM information_schema.referential_constraints
            WHERE DELETE_RULE = 'CASCADE' AND CONSTRAINT_NAME = 'FKShoppingCartsCustomerName'");

        var constraintCount = await command.ExecuteScalarAsync<int>();
        Assert.Equal(1, constraintCount);
    }
}
