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
    public class GetChannelEventTest
    {
        private LiveStreamFixture _fixture;
        private readonly GetChannelEventSample _getSample;

        public GetChannelEventTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _getSample = new GetChannelEventSample();
        }

        [Fact]
        public void GetsChannelEvent()
        {
            var result = _getSample.GetChannelEvent(
                _fixture.ProjectId, _fixture.LocationId, _fixture.TestChannelForGetListEventId, _fixture.TestEventId);
            Assert.Equal(_fixture.ProjectId, result.EventName.ProjectId);
            Assert.Equal(_fixture.LocationId, result.EventName.LocationId);
            Assert.Equal(_fixture.TestEventId, result.EventName.EventId);
        }
    }
}