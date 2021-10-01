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
    public class ListClustersTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateClusterSample _createClusterSample;
        private readonly CreateRealmSample _createRealmSample;
        private readonly ListClustersSample _listSample;

        private string _clusterId;
        private string _realmId;

        public ListClustersTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createClusterSample = new CreateClusterSample();
            _createRealmSample = new CreateRealmSample();
            _listSample = new ListClustersSample();
            _realmId = $"test-realm-{_fixture.RandomId()}";
            _clusterId = $"test-cluster-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createRealmSample.CreateRealm(
                    _fixture.ProjectId, _fixture.RegionId,
                    _realmId);
            _fixture.RealmIds.Add(_realmId);

            await _createClusterSample.CreateCluster(
                _fixture.ProjectId, _fixture.RegionId, _realmId,
                _clusterId, _fixture.GkeClusterName);
            _fixture.ClusterIdentifiers.Add(new ClusterIdentifierUtil(_realmId, _clusterId));
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void ListsClusters()
        {
            var clusters = _listSample.ListClusters(_fixture.ProjectId, _fixture.RegionId, _realmId);
            Assert.Contains(clusters, c => _clusterId == c.GameServerClusterName.ClusterId);
        }
    }
}
