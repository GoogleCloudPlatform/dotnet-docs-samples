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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class QueryDataWithJsonParameterAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public QueryDataWithJsonParameterAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestQueryDataWithJsonParameterAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            await _spannerFixture.CreateVenuesTableAndInsertDataAsync(databaseId);

            AddJsonColumnAsyncSample addColumnSample = new AddJsonColumnAsyncSample();
            await addColumnSample.AddJsonColumnAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            var updateJsonSample = new UpdateDataWithJsonAsyncSample();
            await _spannerFixture.Retryable.Eventually(async () =>
                await updateJsonSample.UpdateDataWithJsonAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId));

            var queryJsonSample = new QueryDataWithJsonParameterAsyncSample();
            var venues = await queryJsonSample.QueryDataWithJsonParameterAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);
            Assert.Contains(venues, v => v.VenueId == 19);
        });
    }
}
