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
    public class UpdateRolloutRemoveOverrideConfigAsyncTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateConfigSample _createConfigSample;
        private readonly CreateDeploymentSample _createDeploymentSample;
        private readonly GetRolloutSample _getSample;
        private readonly UpdateRolloutOverrideConfigSample _updateAddOverrideSample;
        private readonly UpdateRolloutRemoveOverrideConfigSample _updateRemoveOverrideSample;
        private string _configId;
        private string _deploymentId;

        public UpdateRolloutRemoveOverrideConfigAsyncTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _getSample = new GetRolloutSample();
            _updateAddOverrideSample = new UpdateRolloutOverrideConfigSample();
            _updateRemoveOverrideSample = new UpdateRolloutRemoveOverrideConfigSample();

            _createDeploymentSample = new CreateDeploymentSample();
            _deploymentId = $"{_fixture.DeploymentIdPrefix}-{_fixture.RandomId()}";
            _fixture.DeploymentIds.Add(_deploymentId);
            _createConfigSample = new CreateConfigSample();
            _configId = $"{_fixture.ConfigIdPrefix}-{_fixture.RandomId()}";
            _fixture.ConfigIdentifiers.Add(new ConfigIdentifier(_deploymentId, _configId));
        }

        public async Task InitializeAsync()
        {
            await _createDeploymentSample.CreateDeploymentAsync(
                    _fixture.ProjectId, _deploymentId);
            await _createConfigSample.CreateConfigAsync(
                    _fixture.ProjectId, _fixture.RegionId, _deploymentId,
                    _configId);
            await _updateAddOverrideSample.UpdateRolloutOverrideConfigAsync(_fixture.ProjectId, _deploymentId, _configId, _fixture.RegionId, _fixture.TestRealmId);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public async Task UpdateRolloutRemoveOverrideConfigAsync()
        {
            await _updateRemoveOverrideSample.UpdateRolloutRemoveOverrideConfigAsync(_fixture.ProjectId, _deploymentId);

            var rollout = _getSample.GetRollout(_fixture.ProjectId, _deploymentId);
            Assert.Equal(0, rollout.GameServerConfigOverrides.Count);
        }
    }
}