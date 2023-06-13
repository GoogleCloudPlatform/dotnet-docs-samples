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
public class CreateTableWithForeignKeyDeleteCascadeAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateTableWithForeignKeyDeleteCascadeAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task CreateTableWithForeignKeyDeleteCascadeAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var sample = new CreateTableWithForeignKeyDeleteCascadeAsyncSample();
            await sample.CreateTableWithForeignKeyDeleteCascadeAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            await AssertTablesCreatedAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
        });
    }

    private async Task AssertTablesCreatedAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);

        var command = connection.CreateSelectCommand("SELECT Count(*) FROM information_schema.tables WHERE table_name='Customers' OR table_name='ShoppingCarts'");
        var tableCount = await command.ExecuteScalarAsync<int>();
        Assert.Equal(2, tableCount);
    }
}
