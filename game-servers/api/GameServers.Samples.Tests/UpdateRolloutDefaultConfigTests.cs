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
    public class UpdateRolloutDefaultConfigTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateDeploymentSample _createDeploymentSample;
        private readonly CreateConfigSample _createConfigSample;
        private readonly GetRolloutSample _getSample;
        private readonly UpdateRolloutDefaultConfigSample _updateSample;
        private readonly UpdateRolloutRemoveDefaultConfigSample _updateRemoveSample;
        private string _configId;
        private string _deploymentId;

        public UpdateRolloutDefaultConfigTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createDeploymentSample = new CreateDeploymentSample();
            _createConfigSample = new CreateConfigSample();
            _getSample = new GetRolloutSample();
            _updateSample = new UpdateRolloutDefaultConfigSample();
            _updateRemoveSample = new UpdateRolloutRemoveDefaultConfigSample();
            _configId = $"test-config-{_fixture.RandomId()}";
            _deploymentId = $"test-deployment-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createDeploymentSample.CreateDeployment(
                    _fixture.ProjectId, _deploymentId);
            _fixture.DeploymentIds.Add(_deploymentId);

            await _createConfigSample.CreateConfig(
                _fixture.ProjectId, _fixture.RegionId, _deploymentId,
                _configId);
            _fixture.ConfigIdentifiers.Add(new ConfigIdentifierUtil(_deploymentId, _configId));
        }

        public async Task DisposeAsync()
        {
            await _updateRemoveSample.UpdateRolloutRemoveDefaultConfig(_fixture.ProjectId, _deploymentId);
        }

        [Fact]
        public async void UpdatesRolloutDefault()
        {
            await _updateSample.UpdateRolloutDefaultConfig(_fixture.ProjectId, _deploymentId, _configId);

            var rollout = _getSample.GetRollout(_fixture.ProjectId, _deploymentId);
            var fullConfigId = $"projects/{_fixture.ProjectId}/locations/global/gameServerDeployments/{_deploymentId}/configs/{_configId}";
            Assert.Equal(fullConfigId, rollout.DefaultGameServerConfig);
        }
    }
}
