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
    public class UpdateClusterTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateClusterSample _createClusterSample;
        private readonly CreateRealmSample _createRealmSample;
        private readonly GetClusterSample _getSample;
        private readonly UpdateClusterSample _updateSample;

        private string _clusterId;
        private string _realmId;

        public UpdateClusterTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createClusterSample = new CreateClusterSample();
            _createRealmSample = new CreateRealmSample();
            _getSample = new GetClusterSample();
            _updateSample = new UpdateClusterSample();
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
        public async void UpdatesCluster()
        {
            await _updateSample.UpdateCluster(_fixture.ProjectId, _fixture.RegionId, _realmId, _clusterId);

            _fixture.ResourcePoller.Eventually(() =>
            {
                var cluster = _getSample.GetCluster(_fixture.ProjectId, _fixture.RegionId, _realmId, _clusterId);
                string value1;
                string value2;
                Assert.True(cluster.Labels.TryGetValue(_fixture.Label1Key, out value1));
                Assert.True(cluster.Labels.TryGetValue(_fixture.Label2Key, out value2));
                Assert.Equal(_fixture.Label1Value, value1);
                Assert.Equal(_fixture.Label2Value, value2);
            });
        }
    }
}
