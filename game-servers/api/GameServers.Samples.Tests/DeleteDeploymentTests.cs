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
    public class DeleteDeploymentTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateDeploymentSample _createSample;
        private readonly DeleteDeploymentSample _deleteSample;
        private string _deploymentId;

        public DeleteDeploymentTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateDeploymentSample();
            _deleteSample = new DeleteDeploymentSample();
            _deploymentId = $"{_fixture.DeploymentIdPrefix}-{_fixture.RandomId()}";
            _fixture.DeploymentIds.Add(_deploymentId);
        }

        public async Task InitializeAsync()
        {
            await _createSample.CreateDeploymentAsync(
                    _fixture.ProjectId, _deploymentId);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void DeletesDeployment()
        {
            _deleteSample.DeleteDeployment(_fixture.ProjectId, _deploymentId);
        }
    }
}