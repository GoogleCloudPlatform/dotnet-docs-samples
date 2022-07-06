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
    public class CreateRealmAsyncTest
    {
        private GameServersFixture _fixture;
        private readonly CreateRealmSample _createSample;
        private string _realmId;

        public CreateRealmAsyncTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateRealmSample();
            _realmId = $"{_fixture.RealmIdPrefix}-{_fixture.RandomId()}";
            _fixture.RealmIds.Add(_realmId);
        }

        [Fact]
        public async Task CreatesRealmAsync()
        {
            var result = await _createSample.CreateRealmAsync(
                _fixture.ProjectId, _fixture.RegionId, _realmId);

            Assert.Equal(_fixture.ProjectId, result.RealmName.ProjectId);
            Assert.Equal(_fixture.RegionId, result.RealmName.LocationId);
            Assert.Equal(_realmId, result.RealmName.RealmId);
        }
    }
}