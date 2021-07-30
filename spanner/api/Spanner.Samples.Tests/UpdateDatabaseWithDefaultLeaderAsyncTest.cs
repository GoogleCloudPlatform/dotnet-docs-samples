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

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class UpdateDatabaseWithDefaultLeaderAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public UpdateDatabaseWithDefaultLeaderAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestUpdateDatabaseWithDefaultLeaderAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(_spannerFixture.InstanceIdWithMultiRegion, async databaseId =>
        {
            var defaultLeader = "us-central1";
            var sample = new UpdateDatabaseWithDefaultLeaderAsyncSample();
            await sample.UpdateDatabaseWithDefaultLeaderAsync(_spannerFixture.ProjectId,
                _spannerFixture.InstanceIdWithMultiRegion, databaseId, defaultLeader);
                
            var databaseAdminClient = await DatabaseAdminClient.CreateAsync();
            var database = await databaseAdminClient.GetDatabaseAsync(
                DatabaseName.FormatProjectInstanceDatabase(_spannerFixture.ProjectId,
                    _spannerFixture.InstanceIdWithMultiRegion, databaseId));
            Assert.Equal(defaultLeader, database.DefaultLeader);
        });
    }
}
