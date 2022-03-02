// Copyright 2022 Google Inc.
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
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateInterleavedTablesAsyncPostgreTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly CreateInterleavedTablesAsyncPostgreSample _sample;

    public CreateInterleavedTablesAsyncPostgreTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new CreateInterleavedTablesAsyncPostgreSample();
    }

    [Fact]
    public async Task TestCreateInterleavedTablesAsyncPostgre()
    {
        // Act.
        await _sample.CreateInterleavedTablesAsyncPostgre(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        //Assert.
        var tables = await ListTableNamesAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);
        Assert.Collection(tables.OrderBy(j => j),
            item1 => Assert.Equal("albums", item1),
            item2 => Assert.Equal("authors", item2),
            item3 => Assert.Equal("books", item3),
            item4 => Assert.Equal("singers", item4));
    }

    private async Task<List<string>> ListTableNamesAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateSelectCommand("SELECT table_name FROM INFORMATION_SCHEMA.tables WHERE table_schema='public' OR table_schema=''");

        var tableNames = new List<string>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tableNames.Add(reader.GetFieldValue<string>("table_name"));
        }

        return tableNames;
    }
}
