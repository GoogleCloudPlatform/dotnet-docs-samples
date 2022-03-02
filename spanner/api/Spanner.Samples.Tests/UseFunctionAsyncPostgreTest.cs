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
public class UseFunctionAsyncPostgreTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly UseFunctionAsyncPostgreSample _sample;

    public UseFunctionAsyncPostgreTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new UseFunctionAsyncPostgreSample();
    }

    [Fact]
    public async Task TestUseFunctionAsyncPostgre()
    {
        // Act.
        var result = await _sample.UseFunctionAsyncPostgre(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        // Assert.
        Assert.Equal(DateTime.Parse("2010-09-13T04:32:03Z").ToUniversalTime(), result);
    }
}
