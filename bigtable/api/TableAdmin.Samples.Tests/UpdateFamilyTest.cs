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

using Google.Cloud.Bigtable.Admin.V2;
using System;
using Xunit;

[Collection(nameof(BigtableTableAdminFixture))]
public class UpdateFamilyTest
{
    private readonly BigtableTableAdminFixture _fixture;
    public UpdateFamilyTest(BigtableTableAdminFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestUpdateFamily()
    {
        CreateMaxAgeFamilySample createMaxAgeFamilySample = new CreateMaxAgeFamilySample();
        UpdateFamilySample updateFamilySample = new UpdateFamilySample();
        DeleteFamilySample deleteFamilySample = new DeleteFamilySample();
        
        // Create max age family.
        createMaxAgeFamilySample.CreateMaxAgeFamily(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId);

        // Update family.
        var table = updateFamilySample.UpdateFamily(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId);

        Assert.DoesNotContain(table.ColumnFamilies, c => c.Value.GcRule.RuleCase == GcRule.RuleOneofCase.MaxAge);
        
        deleteFamilySample.DeleteFamily(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId, "cf1");
    }
}
