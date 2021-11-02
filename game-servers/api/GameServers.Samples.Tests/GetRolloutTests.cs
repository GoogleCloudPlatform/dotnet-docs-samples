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
    public class GetRolloutTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly GetRolloutSample _getSample;
        private readonly UpdateRolloutDefaultConfigSample _updateSample;
        private readonly UpdateRolloutRemoveDefaultConfigSample _updateRemoveSample;

        public GetRolloutTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _getSample = new GetRolloutSample();
            _updateSample = new UpdateRolloutDefaultConfigSample();
            _updateRemoveSample = new UpdateRolloutRemoveDefaultConfigSample();
        }

        public async Task InitializeAsync()
        {
            await _updateSample.UpdateRolloutDefaultConfigAsync(_fixture.ProjectId, _fixture.TestDeploymentId, _fixture.TestConfigId);
        }

        public async Task DisposeAsync()
        {
            await _updateRemoveSample.UpdateRolloutRemoveDefaultConfigAsync(_fixture.ProjectId, _fixture.TestDeploymentId);
        }

        [Fact]
        public void GetsRollout()
        {
            var rollout = _getSample.GetRollout(_fixture.ProjectId, _fixture.TestDeploymentId);
            var fullConfigId = $"projects/{_fixture.ProjectId}/locations/global/gameServerDeployments/{_fixture.TestDeploymentId}/configs/{_fixture.TestConfigId}";
            Assert.Equal(fullConfigId, rollout.DefaultGameServerConfig);
        }
    }
}