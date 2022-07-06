// Copyright 2021 Google Inc.
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
public class GetDatabaseDdlAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public GetDatabaseDdlAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestGetDatabaseDdlAsync()
    {
        var databaseId = SpannerFixture.GenerateId("my-db-");
        var alterDatabaseStatement = $"ALTER DATABASE `{databaseId}` SET OPTIONS (default_leader = 'us-central1')";
        var createSingersTable =
            @"CREATE TABLE Singers (
                     SingerId INT64 NOT NULL,
                     FirstName STRING(1024),
                     LastName STRING(1024),
                     ComposerInfo BYTES(MAX),
                 ) PRIMARY KEY (SingerId)";
        var createAlbumsTable =
            @"CREATE TABLE Albums (
                     SingerId INT64 NOT NULL,
                     AlbumId INT64 NOT NULL,
                     AlbumTitle STRING(MAX),
                 ) PRIMARY KEY (SingerId, AlbumId),
                 INTERLEAVE IN PARENT Singers ON DELETE CASCADE";
        await _spannerFixture.RunWithTemporaryDatabaseAsync(_spannerFixture.InstanceIdWithMultiRegion, databaseId, async databaseId =>
        {
            var sample = new GetDatabaseDdlAsyncSample();
            var response =
                await sample.GetDatabaseDdlAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceIdWithMultiRegion, databaseId);
            Assert.Collection(response.Statements,
                // Only check the start of the statement, as there is no guarantee on exactly
                // how Cloud Spanner will format the returned SQL string.
                statement => Assert.StartsWith($"ALTER DATABASE `{databaseId}` SET OPTIONS (\n  default_leader = 'us-central1'", statement),
                statement => Assert.StartsWith("CREATE TABLE Singers", statement),
                statement => Assert.StartsWith("CREATE TABLE Albums", statement)
            );
        }, alterDatabaseStatement, createSingersTable, createAlbumsTable);
    }
}
