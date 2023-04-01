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

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class DeleteUsingDmlReturningAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    public DeleteUsingDmlReturningAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestDeleteUsingDmlReturningAsyncPostgres()
    {
        await InsertDataAsync();
        var sample = new DeleteUsingDmlReturningAsyncPostgresSample();
        var deletedSingerNames = await sample.DeleteUsingDmlReturningAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        Assert.Single(deletedSingerNames);
        Assert.Equal("Lata Mangeshkar", deletedSingerNames[0]);
    }

    private async Task InsertDataAsync()
    {
        string dml = "INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (12, 'Lata', 'Mangeshkar')";
        var insertSingerCommand = _spannerFixture.PgSpannerConnection.CreateDmlCommand(dml);
        await insertSingerCommand.ExecuteNonQueryAsync();
    }
}
