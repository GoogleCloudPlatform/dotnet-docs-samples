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

namespace Stitcher.Samples.Tests
{
    [Collection(nameof(StitcherFixture))]
    public class GetLiveSessionTest
    {
        private StitcherFixture _fixture;
        private readonly CreateLiveSessionSample _createSample;
        private readonly GetLiveSessionSample _getSample;

        private string _liveSessionId;

        public GetLiveSessionTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateLiveSessionSample();
            _getSample = new GetLiveSessionSample();

            var result = _createSample.CreateLiveSession(
               _fixture.ProjectId, _fixture.LocationId, _fixture.TestLiveConfigId);
            _liveSessionId = result.LiveSessionName.LiveSessionId;
        }

        [Fact]
        public void GetsLiveSession()
        {
            // Run the sample code.
            var result = _getSample.GetLiveSession(
                _fixture.ProjectId, _fixture.LocationId, _liveSessionId);

            Assert.Equal(_fixture.LocationId, result.LiveSessionName.LocationId);
            Assert.Equal(_liveSessionId, result.LiveSessionName.LiveSessionId);
        }
    }
}
