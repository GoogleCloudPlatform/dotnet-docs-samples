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
    public class ListDeploymentsTest
    {
        private GameServersFixture _fixture;
        private readonly ListDeploymentsSample _listSample;

        public ListDeploymentsTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _listSample = new ListDeploymentsSample();
        }

        [Fact]
        public void ListsDeployments()
        {
            var deployments = _listSample.ListDeployments(_fixture.ProjectId);
            Assert.Contains(deployments, d => _fixture.TestDeploymentId == d.GameServerDeploymentName.DeploymentId);
        }
    }
}