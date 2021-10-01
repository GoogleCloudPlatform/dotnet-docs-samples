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
    public class UpdateRealmTest : IAsyncLifetime
    {
        private GameServersFixture _fixture;
        private readonly CreateRealmSample _createSample;
        private readonly GetRealmSample _getSample;
        private readonly UpdateRealmSample _updateSample;

        private string _realmId;

        public UpdateRealmTest(GameServersFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateRealmSample();
            _getSample = new GetRealmSample();
            _updateSample = new UpdateRealmSample();
            _realmId = $"test-realm-{_fixture.RandomId()}";
        }

        public async Task InitializeAsync()
        {
            await _createSample.CreateRealm(
                    _fixture.ProjectId, _fixture.RegionId,
                    _realmId);
            _fixture.RealmIds.Add(_realmId);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public async void UpdatesRealm()
        {
            await _updateSample.UpdateRealm(_fixture.ProjectId, _fixture.RegionId, _realmId);

            _fixture.ResourcePoller.Eventually(() =>
            {
                var realm = _getSample.GetRealm(_fixture.ProjectId, _fixture.RegionId, _realmId);
                string value1;
                string value2;
                Assert.True(realm.Labels.TryGetValue(_fixture.Label1Key, out value1));
                Assert.True(realm.Labels.TryGetValue(_fixture.Label2Key, out value2));
                Assert.Equal(_fixture.Label1Value, value1);
                Assert.Equal(_fixture.Label2Value, value2);
            });
        }
    }
}
