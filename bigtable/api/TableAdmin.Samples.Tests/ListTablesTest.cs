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

using Xunit;

[Collection(nameof(BigtableTableAdminFixture))]
public class ListTablesTest
{
    private readonly BigtableTableAdminFixture _fixture;
    public ListTablesTest(BigtableTableAdminFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestListTables()
    {
        ListTablesSample listTablesSample = new ListTablesSample();
        var tables = listTablesSample.ListTables(_fixture.ProjectId, _fixture.InstanceId);
        Assert.Contains(tables, t => t.TableName.TableId.Contains(_fixture.TableId));
    }
}
