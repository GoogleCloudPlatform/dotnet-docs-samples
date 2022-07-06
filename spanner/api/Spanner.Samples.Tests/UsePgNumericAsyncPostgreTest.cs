﻿// Copyright 2022 Google Inc.
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

using Google.Cloud.Spanner.V1;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class UsePgNumericAsyncPostgreTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly UsePgNumericAsyncPostgreSample _sample;

    public UsePgNumericAsyncPostgreTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new UsePgNumericAsyncPostgreSample();
    }

    [Fact]
    public async Task TestUsePgNumericAsyncPostgre()
    {
        // Act.
        var result = await _sample.UsePgNumericAsyncPostgre(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        //Assert.
        Assert.Collection(result,
        item1 => Assert.Equal(PgNumeric.Parse("3150.25"), item1.Revenue),
        item2 => Assert.Equal(PgNumeric.Parse("NaN"), item2.Revenue),
        item3 => Assert.Null(item3.Revenue));
    }
}
