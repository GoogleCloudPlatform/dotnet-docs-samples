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

using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class ReadDataWithDatabaseRoleTest
{
    private readonly SpannerFixture _spannerFixture;
    private readonly TimeSpan delay = TimeSpan.FromSeconds(7);

    public ReadDataWithDatabaseRoleTest(SpannerFixture spannerFixture) =>
        _spannerFixture = spannerFixture;

    [Fact]
    public async Task TestReadDataWithDatabaseRoleAsync()
    {
        var readDataWithDatabaseRoleSample = new ReadDataWithDatabaseRoleSample();
        var addAndDropDatabaseRoleSample = new AddAndDropDatabaseRoleSample();
        string dbRole = "readerRole";
        addAndDropDatabaseRoleSample.AddDatabaseRole(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId, dbRole);
        await Task.Delay(delay);

        string grantAccessStatement = $"GRANT SELECT ON TABLE Venues TO ROLE {dbRole}";
        using var updateCmd = _spannerFixture.SpannerConnection.CreateDdlCommand(grantAccessStatement);
        await updateCmd.ExecuteNonQueryAsync();
        await Task.Delay(delay);

        var venues = await readDataWithDatabaseRoleSample.ReadDataWithDatabaseRoleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId, dbRole);
        Assert.NotEmpty(venues);
    }
}
