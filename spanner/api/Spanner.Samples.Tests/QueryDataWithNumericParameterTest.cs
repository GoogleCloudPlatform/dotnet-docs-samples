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
public class QueryDataWithNumericParameterTest
{
    private readonly SpannerFixture _spannerFixture;

    public QueryDataWithNumericParameterTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestQueryDataWithNumericParameter()
    {
        UpdateDataWithNumericAsyncSample updateNumericSample = new UpdateDataWithNumericAsyncSample();
        await updateNumericSample.UpdateDataWithNumericAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId);

        QueryDataWithNumericParameterAsyncSample queryNumericSample = new QueryDataWithNumericParameterAsyncSample();
        var venues = await queryNumericSample.QueryDataWithNumericParameterAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.DatabaseId);
        Assert.Contains(venues, v => v.VenueId == 4);
    }
}
