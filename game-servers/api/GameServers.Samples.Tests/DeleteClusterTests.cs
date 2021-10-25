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
    public class DeleteClusterTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateClusterSample _createClusterSample;
        private readonly DeleteClusterSample _deleteSample;
        private string _clusterId;

        public DeleteClusterTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createClusterSample = new CreateClusterSample();
            _deleteSample = new DeleteClusterSample();
            _clusterId = $"{_fixture.ClusterIdPrefix}-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createClusterSample.CreateClusterAsync(
                _fixture.ProjectId, _fixture.RegionId, _fixture.TestRealmId,
                _clusterId, _fixture.GkeClusterName);
            _fixture.ClusterIdentifiers.Add(new ClusterIdentifier(_fixture.TestRealmId, _clusterId));
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void DeletesCluster()
        {
            _deleteSample.DeleteCluster(_fixture.ProjectId, _fixture.RegionId, _fixture.TestRealmId, _clusterId);
        }
    }
}