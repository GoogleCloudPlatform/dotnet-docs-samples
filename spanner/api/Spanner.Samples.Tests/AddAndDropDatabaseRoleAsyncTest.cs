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

using System.Linq;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class AddAndDropDatabaseRoleAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public AddAndDropDatabaseRoleAsyncTest(SpannerFixture spannerFixture) =>
        _spannerFixture = spannerFixture;

    [Fact]
    public async Task TestAddDatabaseRole()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var addAndDropDatabaseRoleSample = new AddAndDropDatabaseRoleAsyncSample();
            var listDatabaseRolesSample = new ListDatabaseRolesSample();
            string testDbRole = "testRole";

            var oldDbRoles = listDatabaseRolesSample.ListDatabaseRoles(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            int oldDbRolesCount = oldDbRoles.Select(res => res.DatabaseRoleName).Count();

            await addAndDropDatabaseRoleSample.AddDatabaseRoleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId, testDbRole);
            var dbRolesAfterAddition = listDatabaseRolesSample.ListDatabaseRoles(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            int dbRolesAfterAdditionCount = dbRolesAfterAddition.Select(res => res.DatabaseRoleName).Count();

            Assert.Equal(oldDbRolesCount + 1, dbRolesAfterAdditionCount);
        });
    }

    [Fact]
    public async Task TestDropDatabaseRole()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var addAndDropDatabaseRoleSample = new AddAndDropDatabaseRoleAsyncSample();
            var listDatabaseRolesSample = new ListDatabaseRolesSample();
            string testDbRole = "testRole";

            await addAndDropDatabaseRoleSample.AddDatabaseRoleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId, testDbRole);
            var dbRolesAfterAddition = listDatabaseRolesSample.ListDatabaseRoles(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            int dbRolesAfterAdditionCount = dbRolesAfterAddition.Select(res => res.DatabaseRoleName).Count();

            await addAndDropDatabaseRoleSample.DropDatabaseRoleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId, testDbRole);
            var dbRolesAfterDeletion = listDatabaseRolesSample.ListDatabaseRoles(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            int dbRolesAfterDeletionCount = dbRolesAfterDeletion.Select(res => res.DatabaseRoleName).Count();

            Assert.Equal(dbRolesAfterAdditionCount - 1, dbRolesAfterDeletionCount);
        });
    }
}
