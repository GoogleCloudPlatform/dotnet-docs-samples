/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Xunit;
using System.Threading.Tasks;

namespace GameServers.Samples.Tests
{
    [Collection(nameof(GameServersFixture))]
    public class UpdateClusterAsyncTest
    {
        private GameServersFixture _fixture;
        private readonly GetClusterSample _getSample;
        private readonly UpdateClusterSample _updateSample;

        public UpdateClusterAsyncTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _getSample = new GetClusterSample();
            _updateSample = new UpdateClusterSample();
        }

        [Fact]
        public async Task UpdatesClusterAsync()
        {
            await _updateSample.UpdateClusterAsync(_fixture.ProjectId, _fixture.RegionId, _fixture.TestRealmId, _fixture.TestClusterId);

            var cluster = _getSample.GetCluster(_fixture.ProjectId, _fixture.RegionId, _fixture.TestRealmId, _fixture.TestClusterId);
            string value1;
            string value2;
            Assert.True(cluster.Labels.TryGetValue(_fixture.Label1Key, out value1));
            Assert.True(cluster.Labels.TryGetValue(_fixture.Label2Key, out value2));
            Assert.Equal(_fixture.Label1Value, value1);
            Assert.Equal(_fixture.Label2Value, value2);
        }
    }
}