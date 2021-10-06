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
    public class CreateConfigTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateConfigSample _createConfigSample;
        private readonly CreateDeploymentSample _createDeploymentSample;
        private string _configId;
        private string _deploymentId;

        public CreateConfigTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createConfigSample = new CreateConfigSample();
            _createDeploymentSample = new CreateDeploymentSample();
            _configId = $"test-config-{_fixture.RandomId()}";
            _deploymentId = $"test-deployment-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createDeploymentSample.CreateDeployment(
                    _fixture.ProjectId, _deploymentId);
            _fixture.DeploymentIds.Add(_deploymentId);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public async void CreatesConfig()
        {
            var result = await _createConfigSample.CreateConfig(
                _fixture.ProjectId, _fixture.RegionId, _deploymentId,
                _configId);
            _fixture.ConfigIdentifiers.Add(new ConfigIdentifierUtil(_deploymentId, _configId));

            Assert.Equal(_fixture.ProjectId, result.GameServerConfigName.ProjectId);
            Assert.Equal(_fixture.RegionId, result.GameServerConfigName.LocationId);
            Assert.Equal(_deploymentId, result.GameServerConfigName.DeploymentId);
            Assert.Equal(_configId, result.GameServerConfigName.ConfigId);
        }
    }
}
