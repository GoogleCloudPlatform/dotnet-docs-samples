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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class InsertUsingBatchDmlAsyncPostgresTest
{
    private readonly SpannerFixture _spannerFixture;

    private readonly InsertUsingBatchDmlAsyncPostgresSample _sample;

    public InsertUsingBatchDmlAsyncPostgresTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
        _sample = new InsertUsingBatchDmlAsyncPostgresSample();
    }

    [Fact]
    public async Task TestInsertUsingBatchDmlAsyncPostgres()
    {
        await _spannerFixture.RunWithTemporaryPostgresDatabaseAsync(async databaseId =>
        {
            // Act.
            var count = await _sample.InsertUsingBatchDmlAsyncPostgres(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            // Assert.
            Assert.True(count >= 3);
        });
    }
}
