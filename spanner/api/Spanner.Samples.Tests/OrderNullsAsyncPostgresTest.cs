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

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class OrderNullsAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly OrderNullsAsyncPostgresSample _sample;

    public OrderNullsAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new OrderNullsAsyncPostgresSample();
    }

    [Fact]
    public async Task TestOrderNullsAsyncPostgres()
    {
        // Arrange.
        await CreateTableForOrderingAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        // Act.
        var result = await _sample.OrderNullsAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        //Assert.
        Assert.Collection(result,
            // Alice, Bruce, null.
            item1 => Assert.Equal("Alice", item1),
            item2 => Assert.Equal("Bruce", item2),
            item3 => Assert.Null(item3),

            // null, Bruce, Alice.
            item4 => Assert.Null(item4),
            item5 => Assert.Equal("Bruce", item5),
            item6 => Assert.Equal("Alice", item6),

            // null, Alice, Bruce.
            item7 => Assert.Null(item7),
            item8 => Assert.Equal("Alice", item8),
            item9 => Assert.Equal("Bruce", item9),

            // Bruce, Alice, null.
            item10 => Assert.Equal("Alice", item10),
            item11 => Assert.Equal("Bruce", item11),
            item12 => Assert.Null(item12));
    }

    private async Task CreateTableForOrderingAsync(string projectId, string instanceId, string databaseId)
    {
        // Create a table.
        var singersTable = @"CREATE TABLE SingersForOrder (
            SingerId bigint NOT NULL PRIMARY KEY,
            Name varchar(1024))";

        DatabaseName databaseName = DatabaseName.FromProjectInstanceDatabase(projectId, instanceId, databaseId);

        // Create UpdateDatabaseRequest to create the table. 
        var updateDatabaseRequest = new UpdateDatabaseDdlRequest
        {
            DatabaseAsDatabaseName = databaseName,
            Statements = { singersTable }
        };

        var updateOperation = await _spannerFixture.DatabaseAdminClient.UpdateDatabaseDdlAsync(updateDatabaseRequest);
        // Wait until the operation has finished.
        Console.WriteLine("Waiting for the table to be created.");
        var updateResponse = await updateOperation.PollUntilCompletedAsync();
        if (updateResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating the table : {updateResponse.Exception}");
            throw updateResponse.Exception;
        }
    }
}
