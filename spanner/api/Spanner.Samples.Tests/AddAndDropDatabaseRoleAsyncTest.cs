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

using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class AddAndDropDatabaseRoleAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public AddAndDropDatabaseRoleAsyncTest(SpannerFixture spannerFixture) =>
        _spannerFixture = spannerFixture;

    [Fact]
    public async Task TestAddAndDropDatabaseRole()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var addAndDropDatabaseRoleSample = new AddAndDropDatabaseRoleAsyncSample();
            var listDatabaseRolesSample = new ListDatabaseRolesSample();
            string testDbRole = "testRole";

            var oldDbRoles = listDatabaseRolesSample.ListDatabaseRoles(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            await addAndDropDatabaseRoleSample.AddDatabaseRoleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId, testDbRole);
            var newDbRolesAfterAddition = listDatabaseRolesSample.ListDatabaseRoles(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            await addAndDropDatabaseRoleSample.DropDatabaseRoleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId, testDbRole);
            var newDbRolesAfterDeletion = listDatabaseRolesSample.ListDatabaseRoles(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            Assert.Equal(oldDbRoles.Count, newDbRolesAfterDeletion.Count);
            Assert.Equal(oldDbRoles.Count + 1, newDbRolesAfterAddition.Count);
        });
    }
}
