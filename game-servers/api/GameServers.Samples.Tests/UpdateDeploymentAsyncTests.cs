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
    public class UpdateDeploymentAsyncTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateDeploymentSample _createSample;
        private readonly GetDeploymentSample _getSample;
        private readonly UpdateDeploymentSample _updateSample;

        private string _deploymentId;

        public UpdateDeploymentAsyncTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateDeploymentSample();
            _getSample = new GetDeploymentSample();
            _updateSample = new UpdateDeploymentSample();
            _deploymentId = $"{_fixture.DeploymentIdPrefix}-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createSample.CreateDeploymentAsync(
                    _fixture.ProjectId, _deploymentId);
            _fixture.DeploymentIds.Add(_deploymentId);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public async Task UpdatesDeploymentAsync()
        {
            await _updateSample.UpdateDeploymentAsync(_fixture.ProjectId, _deploymentId);

            var deployment = _getSample.GetDeployment(_fixture.ProjectId, _deploymentId);
            string value1;
            string value2;
            Assert.True(deployment.Labels.TryGetValue(_fixture.Label1Key, out value1));
            Assert.True(deployment.Labels.TryGetValue(_fixture.Label2Key, out value2));
            Assert.Equal(_fixture.Label1Value, value1);
            Assert.Equal(_fixture.Label2Value, value2);
        }
    }
}
