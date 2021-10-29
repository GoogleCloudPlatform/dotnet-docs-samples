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
    public class CreateDeploymentAsyncTest
    {
        private GameServersFixture _fixture;
        private readonly CreateDeploymentSample _createSample;
        private string _deploymentId;

        public CreateDeploymentAsyncTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateDeploymentSample();
            _deploymentId = $"{_fixture.DeploymentIdPrefix}-{_fixture.RandomId()}";
            _fixture.DeploymentIds.Add(_deploymentId);
        }

        [Fact]
        public async Task CreatesDeploymentAsync()
        {
            var result = await _createSample.CreateDeploymentAsync(
                _fixture.ProjectId, _deploymentId);

            Assert.Equal(_fixture.ProjectId, result.GameServerDeploymentName.ProjectId);
            Assert.Equal("global", result.GameServerDeploymentName.LocationId);
            Assert.Equal(_deploymentId, result.GameServerDeploymentName.DeploymentId);
        }
    }
}