﻿// Copyright 2023 Google Inc.
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class InsertUsingDmlReturningAsyncPostgreTest
{
    private readonly SpannerFixture _spannerFixture;

    public InsertUsingDmlReturningAsyncPostgreTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestInsertUsingDmlReturningAsyncPostgre()
    {
        var sample = new InsertUsingDmlReturningAsyncPostgreSample();
        var insertedSingerNames = await sample.InsertUsingDmlReturningAsyncPostgre(_spannerFixture.ProjectId, _spannerFixture.InstanceId, _spannerFixture.PostgreSqlDatabaseId);

        Assert.Equal(4, insertedSingerNames.Count);
        Assert.Contains("Melissa Garcia", insertedSingerNames);
        Assert.Contains("Russell Morales", insertedSingerNames);
        Assert.Contains("Jacqueline Long", insertedSingerNames);
        Assert.Contains("Dylan Shaw", insertedSingerNames);
    }
}
