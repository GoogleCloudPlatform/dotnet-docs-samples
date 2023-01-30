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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class ReadDataWithDatabaseRoleAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public ReadDataWithDatabaseRoleAsyncTest(SpannerFixture spannerFixture) =>
        _spannerFixture = spannerFixture;

    [Fact]
    public async Task TestReadDataWithDatabaseRoleAsync()
    {
        string dbRole = "readerRole";
        var createSingersTable =
            @"CREATE TABLE Singers (
            SingerId INT64 NOT NULL,
            FirstName STRING(1024),
            LastName STRING(1024)
            ) PRIMARY KEY (SingerId)";
        var createAlbumsTable =
            @"CREATE TABLE Albums (
            SingerId INT64 NOT NULL,
            AlbumId INT64 NOT NULL,
            AlbumTitle STRING(MAX)
            ) PRIMARY KEY (SingerId, AlbumId)";
        string createRoleStatement = $"CREATE ROLE {dbRole}";
        string grantAccessStatement = $"GRANT SELECT ON TABLE Singers TO ROLE {dbRole}";

        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var readDataWithDatabaseRoleSample = new ReadDataWithDatabaseRoleAsyncSample();
            var insertDataAsyncSample = new InsertDataAsyncSample();
            await insertDataAsyncSample.InsertDataAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            var singers = await readDataWithDatabaseRoleSample.ReadDataWithDatabaseRoleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId, dbRole);
            Assert.NotEmpty(singers);
        }, createSingersTable, createAlbumsTable, createRoleStatement, grantAccessStatement);
    }
}
