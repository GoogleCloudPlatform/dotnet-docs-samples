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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class QueryJsonbDataUsingParameterAsyncPostgreTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly QueryJsonbDataUsingParameterAsyncPostgreSample _sample;

    private readonly UpdateDataWithJsonbAsyncPostgreSample _updateDataSample;

    public QueryJsonbDataUsingParameterAsyncPostgreTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new QueryJsonbDataUsingParameterAsyncPostgreSample();
        _updateDataSample = new UpdateDataWithJsonbAsyncPostgreSample();
    }

    [Fact]
    public async Task TestQueryJsonbDataUsingParameterAsyncPostgre()
    {
        // Arrange - Update data.
        await _updateDataSample.UpdateDataWithJsonbAsyncPostgre(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        // Act - Query the VenueInformation table.
        var venues = await _sample.QueryJsonbDataUsingParameterAsyncPostgre(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        // Assert that VenueId 19 exists in the queried data from the table.
        Assert.Contains(venues, v => v.VenueId == 19);
    }
}
