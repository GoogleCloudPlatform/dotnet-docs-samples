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

using System;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class QueryWithDateTest
{
    private readonly SpannerFixture _spannerFixture;

    public QueryWithDateTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async void TestQueryWithDate()
    {
        QueryWithDateAsyncSample sample = new QueryWithDateAsyncSample();
        var venues = await sample.QueryWithDateAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId);
        Assert.Contains(venues, v => v.VenueId == 4 && v.VenueName == "Venue 4" && v.LastContactDate == new DateTime(2018, 09, 02));
        Assert.Contains(venues, v => v.VenueId == 42 && v.VenueName == "Venue 42" && v.LastContactDate == new DateTime(2018, 10, 01));
    }
}
