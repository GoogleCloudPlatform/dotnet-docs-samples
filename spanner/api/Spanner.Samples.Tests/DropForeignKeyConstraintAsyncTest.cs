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
public class DropForeignKeyConstraintAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public DropForeignKeyConstraintAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task DropForeignKeyConstraintAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var createTableWithForeignKeyDeleteCascadeAsyncSample = new CreateTableWithForeignKeyDeleteCascadeAsyncSample();
            await createTableWithForeignKeyDeleteCascadeAsyncSample.CreateTableWithForeignKeyDeleteCascadeAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            var sample = new DropForeignKeyConstraintAsyncSample();
            await sample.DropForeignKeyConstraintAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            await AssertNoForeingKeyConstraintAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
        });
    }

    private async Task AssertNoForeingKeyConstraintAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);

        var command = connection.CreateSelectCommand("SELECT constraint_name FROM information_schema.referential_constraints WHERE delete_rule = 'CASCADE'");

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Assert.Fail($"Found constraint: {reader.GetFieldValue<string>("constraint_name")}");
        }
    }
}
