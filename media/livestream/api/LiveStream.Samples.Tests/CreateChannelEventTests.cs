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
    public class CreateChannelEventTest
    {
        private LiveStreamFixture _fixture;
        private readonly CreateChannelEventSample _createChannelEventSample;
        private string _eventId;

        public CreateChannelEventTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _createChannelEventSample = new CreateChannelEventSample();
            _eventId = $"{_fixture.EventIdPrefix}-{_fixture.RandomId()}";
        }

        [Fact]
        public void CreatesChannelEvent()
        {
            var result = _createChannelEventSample.CreateChannelEvent(
                _fixture.ProjectId, _fixture.LocationId, _fixture.TestChannelForCreateEventId, _eventId);

            Assert.Equal(_fixture.ProjectId, result.EventName.ProjectId);
            Assert.Equal(_fixture.LocationId, result.EventName.LocationId);
            Assert.Equal(_eventId, result.EventName.EventId);
        }
    }
}