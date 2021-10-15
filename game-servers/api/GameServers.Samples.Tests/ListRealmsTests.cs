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
    public class ListRealmsTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateRealmSample _createSample;
        private readonly ListRealmsSample _listSample;

        private string _realmId;

        public ListRealmsTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateRealmSample();
            _listSample = new ListRealmsSample();
            _realmId = $"{_fixture.RealmIdPrefix}-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createSample.CreateRealmAsync(
                    _fixture.ProjectId, _fixture.RegionId,
                    _realmId);
            _fixture.RealmIds.Add(_realmId);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void ListsRealms()
        {
            var realms = _listSample.ListRealms(_fixture.ProjectId, _fixture.RegionId);
            Assert.Contains(realms, r => _realmId == r.RealmName.RealmId);
        }
    }
}
