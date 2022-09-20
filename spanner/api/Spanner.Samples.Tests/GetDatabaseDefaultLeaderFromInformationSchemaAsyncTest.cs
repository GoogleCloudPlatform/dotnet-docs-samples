// Copyright 2021 Google Inc.
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
public class GetDatabaseDefaultLeaderFromInformationSchemaAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public GetDatabaseDefaultLeaderFromInformationSchemaAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestGetDatabaseDefaultLeaderFromInformationSchemaAsync()
    {
        var databaseId = _spannerFixture.GenerateTempDatabaseId();
        var defaultLeader = "us-central1";

        // Create Database with default leader
        var sample = new CreateDatabaseWithDefaultLeaderAsyncSample();
        await sample.CreateDatabaseWithDefaultLeaderAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceIdWithMultiRegion, databaseId, defaultLeader);

        // Get Database default leader
        var getDatabaseDefaultLeaderFromInformationSchemaAsyncSample = new GetDatabaseDefaultLeaderFromInformationSchemaAsyncSample();
        var leader = await getDatabaseDefaultLeaderFromInformationSchemaAsyncSample
            .GetDatabaseDefaultLeaderFromInformationSchemaAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceIdWithMultiRegion, databaseId);

        Assert.Equal(defaultLeader, leader);
    }
}
