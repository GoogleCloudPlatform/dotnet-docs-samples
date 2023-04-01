/*
 * Copyright 2022 Google LLC
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

namespace LiveStream.Samples.Tests
{
    [Collection(nameof(LiveStreamFixture))]
    public class CreateChannelWithBackupInputAsyncTest : IAsyncLifetime
    {
        private LiveStreamFixture _fixture;
        private readonly CreateInputSample _createInputSample;
        private readonly CreateChannelWithBackupInputSample _createChannelSample;
        private string _primaryInputId;
        private string _backupInputId;
        private string _channelId;

        public CreateChannelWithBackupInputAsyncTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _createInputSample = new CreateInputSample();
            _primaryInputId = $"{_fixture.InputIdPrefix}-{_fixture.RandomId()}";
            _fixture.InputIds.Add(_primaryInputId);
            _backupInputId = $"{_fixture.InputIdPrefix}-{_fixture.RandomId()}";
            _fixture.InputIds.Add(_backupInputId);

            _createChannelSample = new CreateChannelWithBackupInputSample();
            _channelId = $"{_fixture.ChannelIdPrefix}-{_fixture.RandomId()}";
            _fixture.ChannelIds.Add(_channelId);
        }

        public async Task InitializeAsync()
        {
            await _createInputSample.CreateInputAsync(_fixture.ProjectId, _fixture.LocationId, _primaryInputId);
            await _createInputSample.CreateInputAsync(_fixture.ProjectId, _fixture.LocationId, _backupInputId);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public async Task CreatesChannelWithBackupInputAsync()
        {
            var result = await _createChannelSample.CreateChannelWithBackupInputAsync(
                _fixture.ProjectId, _fixture.LocationId, _channelId, _primaryInputId, _backupInputId, _fixture.ChannelOutputUri);

            Assert.Equal(_fixture.ProjectId, result.ChannelName.ProjectId);
            Assert.Equal(_fixture.LocationId, result.ChannelName.LocationId);
            Assert.Equal(_channelId, result.ChannelName.ChannelId);
        }
    }
}