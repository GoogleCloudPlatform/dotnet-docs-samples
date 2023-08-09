// Copyright 2023 Google Inc.
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
public class CreateSequencePostgresqlTest
{
    private readonly SpannerFixture _spannerFixture;

    public CreateSequencePostgresqlTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestCreatePostgresqlSequenceAsync()
    {
        await _spannerFixture.RunWithPostgresqlTemporaryDatabaseAsync(async databaseId =>
        {
            var sample = new CreateSequencePostgresqlSample();
            var customerIds = await sample.CreateSequencePostgresqlSampleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            Assert.NotEmpty(customerIds);
            Assert.Equal(3, customerIds.Count);
        });
    }
}
