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

using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class AlterSequenceTest
{
    private readonly SpannerFixture _spannerFixture;

    public AlterSequenceTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestAlterSequenceAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            var createSequenceSample = new CreateSequenceSample();
            await createSequenceSample.CreateSequenceAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            var sample = new AlterSequenceSample();
            var customerIds = await sample.AlterSequenceSampleAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId);

            Assert.All(customerIds, cid => Assert.True(cid < 1000 || cid > 5000000));
        });
    }
}
