﻿// Copyright 2021 Google Inc.
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
public class ListDatabasesTest
{
    private readonly SpannerFixture _spannerFixture;

    public ListDatabasesTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestListDatabases()
    {
        var databaseId = $"my-db-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var defaultLeader = "us-central1";
        var listDatabasesSample = new ListDatabasesSample();
        var createDatabaseWithDefaultLeaderAsyncSample = new CreateDatabaseWithDefaultLeaderAsyncSample();

        // Create Database with Default Leader
        await createDatabaseWithDefaultLeaderAsyncSample
            .CreateDatabaseWithDefaultLeaderAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceIdWithMultiRegion, databaseId, defaultLeader);

        // List databases
        var databases = listDatabasesSample.ListDatabases(_spannerFixture.ProjectId, _spannerFixture.InstanceIdWithMultiRegion);
        Assert.Contains(databases, d => d.DatabaseName.DatabaseId == databaseId && d.DefaultLeader == defaultLeader);
    }
}
