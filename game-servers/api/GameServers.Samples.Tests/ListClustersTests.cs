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

namespace GameServers.Samples.Tests
{
    [Collection(nameof(GameServersFixture))]
    public class ListClustersTest
    {
        private GameServersFixture _fixture;
        private readonly ListClustersSample _listSample;

        public ListClustersTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _listSample = new ListClustersSample();
        }

        [Fact]
        public void ListsClusters()
        {
            var clusters = _listSample.ListClusters(_fixture.ProjectId, _fixture.RegionId, _fixture.TestRealmId);
            Assert.Contains(clusters, c => _fixture.TestClusterId == c.GameServerClusterName.ClusterId);
        }
    }
}