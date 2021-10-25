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
    public class DeleteConfigTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateConfigSample _createConfigSample;
        private readonly DeleteConfigSample _deleteSample;
        private string _configId;

        public DeleteConfigTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createConfigSample = new CreateConfigSample();
            _deleteSample = new DeleteConfigSample();
            _configId = $"{_fixture.ConfigIdPrefix}-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createConfigSample.CreateConfigAsync(
                _fixture.ProjectId, _fixture.RegionId, _fixture.TestDeploymentId,
                _configId);
            _fixture.ConfigIdentifiers.Add(new ConfigIdentifier(_fixture.TestDeploymentId, _configId));
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void DeletesConfig()
        {
            _deleteSample.DeleteConfig(_fixture.ProjectId, _fixture.RegionId, _fixture.TestDeploymentId, _configId);
        }
    }
}