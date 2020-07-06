// Copyright (c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using Xunit;

[Collection(nameof(BigtableTableAdminFixture))]
public class CreateTableTest
{
    private readonly BigtableTableAdminFixture _fixture;

    public CreateTableTest(BigtableTableAdminFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestCreateTable()
    {
        CreateTableSample createTableSample = new CreateTableSample();
        ListTablesSample listTablesSample = new ListTablesSample();
        DeleteTableSample deleteTableSample = new DeleteTableSample();
        var tableId = $"my-table-{Guid.NewGuid().ToString().Substring(0, 8)}";

        createTableSample.CreateTable(_fixture.ProjectId, _fixture.InstanceId, tableId);

        var tables = listTablesSample.ListTables(_fixture.ProjectId, _fixture.InstanceId);

        Assert.Contains(tables, t => t.TableName.TableId.Contains(tableId));

        deleteTableSample.DeleteTable(_fixture.ProjectId, _fixture.InstanceId, tableId);
    }
}
