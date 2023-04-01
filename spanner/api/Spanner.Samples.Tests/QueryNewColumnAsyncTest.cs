// Copyright 2020 Google Inc.
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
public class QueryNewColumnAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public QueryNewColumnAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestQueryNewColumnAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            await _spannerFixture.InitializeTempDatabaseAsync(databaseId);
            await _spannerFixture.RefillMarketingBudgetsAsync(100000, 500000, databaseId);
            QueryNewColumnAsyncSample sample = new QueryNewColumnAsyncSample();
            
            var albums = await sample.QueryNewColumnAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            Assert.Contains(albums, a => a.AlbumId == 1 && a.MarketingBudget == 100000);
            Assert.Contains(albums, a => a.AlbumId == 2 && a.MarketingBudget == 500000);
        }, SpannerFixture.CreateSingersTableStatement, SpannerFixture.CreateAlbumsTableStatement);
    }
}
