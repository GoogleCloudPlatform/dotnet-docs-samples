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
public class AlterSequencePostgresqlTest
{
    private readonly SpannerFixture _spannerFixture;

    public AlterSequencePostgresqlTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestAlterSequencePostgresqlAsync()
    {
        await _spannerFixture.RunWithPostgresqlTemporaryDatabaseAsync(async databaseId =>
        {
            var createSequenceSample = new CreateSequencePostgresqlSample();
            await createSequenceSample.CreateSequencePostgresqlSampleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            var sample = new AlterSequencePostgresqlSample();
            var customerIds = await sample.AlterSequencePostgresqlSampleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            Assert.All(customerIds, cid => Assert.True(cid < 1000 || cid > 5000000));
        });
    }
}
