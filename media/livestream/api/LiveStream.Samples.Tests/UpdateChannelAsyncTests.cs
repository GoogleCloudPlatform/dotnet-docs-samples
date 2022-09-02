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
    public class UpdateChannelAsyncTest
    {
        private LiveStreamFixture _fixture;
        private readonly UpdateChannelSample _updateSample;

        public UpdateChannelAsyncTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _updateSample = new UpdateChannelSample();
        }

        [Fact]
        public async Task UpdatesChannelAsync()
        {
            var result = await _updateSample.UpdateChannelAsync(_fixture.ProjectId, _fixture.LocationId, _fixture.TestChannelId, _fixture.TestUpdateInputId);
            Assert.Equal(_fixture.ProjectId, result.ChannelName.ProjectId);
            Assert.Equal(_fixture.LocationId, result.ChannelName.LocationId);
            Assert.Equal("updated-input", result.InputAttachments[0].Key);
        }
    }
}