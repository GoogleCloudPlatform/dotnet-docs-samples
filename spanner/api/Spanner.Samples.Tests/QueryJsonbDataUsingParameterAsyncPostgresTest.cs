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

using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class QueryJsonbDataUsingParameterAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly QueryJsonbDataUsingParameterAsyncPostgresSample _sample;

    private readonly UpdateDataWithJsonbAsyncPostgresSample _updateDataSample;

    public QueryJsonbDataUsingParameterAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new QueryJsonbDataUsingParameterAsyncPostgresSample();
        _updateDataSample = new UpdateDataWithJsonbAsyncPostgresSample();
    }

    [Fact]
    public async Task TestQueryJsonbDataUsingParameterAsyncPostgres()
    {
        // Arrange - Update data.
        await _updateDataSample.UpdateDataWithJsonbAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        // Act - Query the VenueInformation table.
        var venues = await _sample.QueryJsonbDataUsingParameterAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        // Assert that venues collection is not empty and all the venues in the collection have a rating greater than 2.
        Assert.NotEmpty(venues);
        Assert.All(venues, v => AssertRating(v.Details, 2));
    }

    // Asserts that the rating of the given venue details is greater than the threshold value.
    private static void AssertRating(string details, int threshold)
    {
        var venueDetails = JsonConvert.DeserializeObject<Details>(details);
        Assert.True(venueDetails.Rating > threshold);
    }

    private struct Details
    {
        public int Rating { get; set; }
    }
}
