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
    public class GetClusterTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateClusterSample _createClusterSample;
        private readonly CreateRealmSample _createRealmSample;
        private readonly GetClusterSample _getSample;

        private string _clusterId;
        private string _realmId;

        public GetClusterTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createClusterSample = new CreateClusterSample();
            _createRealmSample = new CreateRealmSample();
            _getSample = new GetClusterSample();
            _realmId = $"{_fixture.RealmIdPrefix}-{_fixture.RandomId()}";
            _clusterId = $"{_fixture.ClusterIdPrefix}-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createRealmSample.CreateRealmAsync(
                    _fixture.ProjectId, _fixture.RegionId,
                    _realmId);
            _fixture.RealmIds.Add(_realmId);

            await _createClusterSample.CreateClusterAsync(
                _fixture.ProjectId, _fixture.RegionId, _realmId,
                _clusterId, _fixture.GkeClusterName);
            _fixture.ClusterIdentifiers.Add(new ClusterIdentifier(_realmId, _clusterId));
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void GetsCluster()
        {
            var result = _getSample.GetCluster(_fixture.ProjectId, _fixture.RegionId, _realmId, _clusterId);
            Assert.Equal(_fixture.ProjectId, result.GameServerClusterName.ProjectId);
            Assert.Equal(_fixture.RegionId, result.GameServerClusterName.LocationId);
            Assert.Equal(_clusterId, result.GameServerClusterName.ClusterId);
        }
    }
}
