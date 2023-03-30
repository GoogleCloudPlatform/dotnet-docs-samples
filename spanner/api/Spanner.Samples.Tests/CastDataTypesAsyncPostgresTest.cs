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

using System;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class CastDataTypesAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly CastDataTypesAsyncPostgresSample _sample;

    public CastDataTypesAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new CastDataTypesAsyncPostgresSample();
    }

    [Fact]
    public async Task TestCastDataTypesAsyncPostgres()
    {
        // Act.
        var result = await _sample.CastDataTypesAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        //Assert.
        Assert.Equal("1", result.String);
        Assert.Equal(2, result.Integer);
        Assert.Equal(3, result.Decimal);
        Assert.Equal("34", BitConverter.ToString(result.Bytes));
        Assert.Equal(5.00, result.Float);
        Assert.True(result.Bool);
        Assert.Equal("2021-11-03 09:35:01Z", result.Timestamp.ToString("u"));
    }
}
