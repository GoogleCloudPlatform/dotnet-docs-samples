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
public class CreateNestedFamilyTest
{
    private readonly BigtableTableAdminFixture _fixture;
    public CreateNestedFamilyTest(BigtableTableAdminFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestCreateNestedFamily()
    {
        CreateNestedFamilySample createNestedFamilySample = new CreateNestedFamilySample();
        DeleteFamilySample deleteFamilySample = new DeleteFamilySample();
        var table = createNestedFamilySample.CreateNestedFamily(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId);

        Assert.Contains(table.ColumnFamilies, t => t.Value.GcRule.Union != null);
        deleteFamilySample.DeleteFamily(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId, "cf5");
    }
}
