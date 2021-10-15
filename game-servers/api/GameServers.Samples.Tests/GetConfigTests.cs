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
    public class GetConfigTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateConfigSample _createConfigSample;
        private readonly CreateDeploymentSample _createDeploymentSample;
        private readonly GetConfigSample _getSample;
        private string _configId;
        private string _deploymentId;

        public GetConfigTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createConfigSample = new CreateConfigSample();
            _createDeploymentSample = new CreateDeploymentSample();
            _getSample = new GetConfigSample();
            _configId = $"{_fixture.ConfigIdPrefix}-{_fixture.RandomId()}";
            _deploymentId = $"{_fixture.DeploymentIdPrefix}-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createDeploymentSample.CreateDeploymentAsync(
                    _fixture.ProjectId, _deploymentId);
            _fixture.DeploymentIds.Add(_deploymentId);

            await _createConfigSample.CreateConfigAsync(
                _fixture.ProjectId, _fixture.RegionId, _deploymentId,
                _configId);
            _fixture.ConfigIdentifiers.Add(new ConfigIdentifier(_deploymentId, _configId));
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void GetsConfig()
        {
            var result = _getSample.GetConfig(_fixture.ProjectId, _fixture.RegionId, _deploymentId, _configId);
            Assert.Equal(_fixture.ProjectId, result.GameServerConfigName.ProjectId);
            Assert.Equal(_fixture.RegionId, result.GameServerConfigName.LocationId);
            Assert.Equal(_deploymentId, result.GameServerConfigName.DeploymentId);
            Assert.Equal(_configId, result.GameServerConfigName.ConfigId);
        }
    }
}
