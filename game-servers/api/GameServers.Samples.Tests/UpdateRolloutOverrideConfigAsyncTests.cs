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
using System;

namespace GameServers.Samples.Tests
{
    [Collection(nameof(GameServersFixture))]
    public class UpdateRolloutOverrideConfigAsyncTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateDeploymentSample _createDeploymentSample;
        private readonly CreateConfigSample _createConfigSample;
        private readonly CreateRealmSample _createRealmSample;
        private readonly GetRolloutSample _getSample;
        private readonly UpdateRolloutOverrideConfigSample _updateAddOverrideSample;
        private readonly UpdateRolloutRemoveOverrideConfigSample _updateRemoveOverrideSample;
        private string _configId;
        private string _deploymentId;
        private string _realmId;

        public UpdateRolloutOverrideConfigAsyncTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createDeploymentSample = new CreateDeploymentSample();
            _createConfigSample = new CreateConfigSample();
            _createRealmSample = new CreateRealmSample();
            _getSample = new GetRolloutSample();
            _updateAddOverrideSample = new UpdateRolloutOverrideConfigSample();
            _updateRemoveOverrideSample = new UpdateRolloutRemoveOverrideConfigSample();
            _configId = $"{_fixture.ConfigIdPrefix}-{_fixture.RandomId()}";
            _deploymentId = $"{_fixture.DeploymentIdPrefix}-{_fixture.RandomId()}";
            _realmId = $"{_fixture.RealmIdPrefix}-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            // Tests a global realm.
            await _createRealmSample.CreateRealmAsync(
                    _fixture.ProjectId, _fixture.RegionId,
                    _realmId);
            _fixture.RealmIds.Add(_realmId);

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
            await _updateRemoveOverrideSample.UpdateRolloutRemoveOverrideConfigAsync(_fixture.ProjectId, _deploymentId);
        }

        [Fact]
        public async Task UpdatesRolloutOverrideConfigAsync()
        {
            await _updateAddOverrideSample.UpdateRolloutOverrideConfigAsync(_fixture.ProjectId, _deploymentId, _configId, _fixture.RegionId, _realmId);

            var rollout = _getSample.GetRollout(_fixture.ProjectId, _deploymentId);
            var fullConfigId = $"projects/{_fixture.ProjectId}/locations/global/gameServerDeployments/{_deploymentId}/configs/{_configId}";
            var fullRealmId = $"projects/{_fixture.ProjectId}/locations/{_fixture.RegionId}/realms/{_realmId}";
            Assert.Equal(fullConfigId, rollout.GameServerConfigOverrides[0].ConfigVersion);
            Assert.Equal(fullRealmId, rollout.GameServerConfigOverrides[0].RealmsSelector.Realms[0]);
        }
    }
}
