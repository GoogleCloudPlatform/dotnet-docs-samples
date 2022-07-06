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
    public class GetDeploymentTest
    {
        private GameServersFixture _fixture;
        private readonly GetDeploymentSample _getSample;

        public GetDeploymentTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _getSample = new GetDeploymentSample();
        }

        [Fact]
        public void GetsDeployment()
        {
            var result = _getSample.GetDeployment(_fixture.ProjectId, _fixture.TestDeploymentId);
            Assert.Equal(_fixture.ProjectId, result.GameServerDeploymentName.ProjectId);
            Assert.Equal(_fixture.RegionId, result.GameServerDeploymentName.LocationId);
            Assert.Equal(_fixture.TestDeploymentId, result.GameServerDeploymentName.DeploymentId);
        }
    }
}