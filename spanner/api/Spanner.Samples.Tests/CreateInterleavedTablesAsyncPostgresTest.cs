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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CreateInterleavedTablesAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly CreateInterleavedTablesAsyncPostgresSample _sample;

    public CreateInterleavedTablesAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new CreateInterleavedTablesAsyncPostgresSample();
    }

    [Fact]
    public async Task TestCreateInterleavedTablesAsyncPostgres()
    {
        // Act.
        await _sample.CreateInterleavedTablesAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        // Assert that both interleaved tables are created.
        var tables = await ListTableNamesAsync();
        Assert.Contains("authors", tables);
        Assert.Contains("books", tables);
    }

    private async Task<List<string>> ListTableNamesAsync()
    {
        var command = _spannerFixture.PgSpannerConnection.CreateSelectCommand("SELECT table_name FROM INFORMATION_SCHEMA.tables WHERE table_schema='public' OR table_schema=''");

        var tableNames = new List<string>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tableNames.Add(reader.GetFieldValue<string>("table_name"));
        }

        return tableNames;
    }
}
