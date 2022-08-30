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
    public class DeleteChannelAsyncTest : IAsyncLifetime
    {
        private LiveStreamFixture _fixture;
        private readonly CreateInputSample _createInputSample;
        private readonly CreateChannelSample _createChannelSample;
        private readonly DeleteChannelSample _deleteChannelSample;
        private string _inputId;
        private string _channelId;

        public DeleteChannelAsyncTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _createInputSample = new CreateInputSample();
            _inputId = $"{_fixture.InputIdPrefix}-{_fixture.RandomId()}";
            _fixture.InputIds.Add(_inputId);

            _createChannelSample = new CreateChannelSample();
            _channelId = $"{_fixture.ChannelIdPrefix}-{_fixture.RandomId()}";
            _fixture.ChannelIds.Add(_channelId);

            _deleteChannelSample = new DeleteChannelSample();
        }

        public async Task InitializeAsync()
        {
            await _createInputSample.CreateInputAsync(
                _fixture.ProjectId, _fixture.LocationId, _inputId);

            await _createChannelSample.CreateChannelAsync(
               _fixture.ProjectId, _fixture.LocationId, _channelId, _inputId, _fixture.ChannelOutputUri);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public async Task DeletesChannelAsync()
        {
            await _deleteChannelSample.DeleteChannelAsync(
                _fixture.ProjectId, _fixture.LocationId, _channelId);
        }
    }
}