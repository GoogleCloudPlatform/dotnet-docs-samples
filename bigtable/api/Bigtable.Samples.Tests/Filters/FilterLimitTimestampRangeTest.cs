// Copyright (c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using Xunit;

[Collection(nameof(BigtableClientFixture))]
public class FilterLimitTimestampRangeTest
{
    private readonly BigtableClientFixture _fixture;

    public FilterLimitTimestampRangeTest(BigtableClientFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async void TestFilterLimitTimestampRange()
    {
        FilterLimitTimestampRangeAsyncSample filterLimitTimestampRangeAsyncSample = new FilterLimitTimestampRangeAsyncSample();
        var timestamp = new DateTime(2020, 1, 10, 14, 0, 0, DateTimeKind.Utc);
        var timestamp_minus_hr = new DateTime(2020, 1, 10, 13, 0, 0, DateTimeKind.Utc);
        var result = await filterLimitTimestampRangeAsyncSample.FilterLimitTimestampRangeAsync(_fixture.ProjectId, _fixture.InstanceId, _fixture.TableId, timestamp_minus_hr, timestamp);
        Assert.True(result.Count >= 1);
    }
}
