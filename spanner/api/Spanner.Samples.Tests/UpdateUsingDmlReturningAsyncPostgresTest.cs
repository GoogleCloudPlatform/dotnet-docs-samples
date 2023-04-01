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
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class UpdateUsingDmlReturningAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    public UpdateUsingDmlReturningAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestUpdateUsingDmlReturningAsyncPostgres()
    {
        await InsertDataAsync();
        var sample = new UpdateUsingDmlReturningAsyncPostgresSample();
        var updatedMarketingBudgets = await sample.UpdateUsingDmlReturningAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        Assert.Single(updatedMarketingBudgets);
        Assert.Equal(20000, updatedMarketingBudgets[0]);
    }

    private async Task InsertDataAsync()
    {
        var batchCommand = _spannerFixture.PgSpannerConnection.CreateBatchDmlCommand();
        batchCommand.Add("INSERT INTO Singers (SingerId, FirstName, LastName) VALUES (14, 'Kishore', 'Kumar')");
        batchCommand.Add("INSERT INTO Albums(SingerId, AlbumId, AlbumTitle, MarketingBudget) VALUES (14, 20, 'Test Album Title', 10000)");
        await batchCommand.ExecuteNonQueryAsync();
    }
}
