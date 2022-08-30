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

namespace LiveStream.Samples.Tests
{
    [Collection(nameof(LiveStreamFixture))]
    public class GetChannelTest
    {
        private LiveStreamFixture _fixture;
        private readonly GetChannelSample _getSample;

        public GetChannelTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _getSample = new GetChannelSample();
        }

        [Fact]
        public void GetsChannel()
        {
            var result = _getSample.GetChannel(
                _fixture.ProjectId, _fixture.LocationId, _fixture.TestChannelId);
            Assert.Equal(_fixture.ProjectId, result.ChannelName.ProjectId);
            Assert.Equal(_fixture.LocationId, result.ChannelName.LocationId);
            Assert.Equal(_fixture.TestChannelId, result.ChannelName.ChannelId);
        }
    }
}