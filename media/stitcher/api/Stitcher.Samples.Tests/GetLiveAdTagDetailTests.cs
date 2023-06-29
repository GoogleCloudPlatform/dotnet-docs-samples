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

using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Stitcher.Samples.Tests
{
    [Collection(nameof(StitcherFixture))]
    public class GetLiveAdTagDetailTest : IAsyncLifetime
    {
        private StitcherFixture _fixture;
        private readonly CreateLiveSessionSample _createSample;
        private readonly GetLiveAdTagDetailSample _getSample;
        private readonly ListLiveAdTagDetailsSample _listSample;

        private string _liveSessionId;
        private string _adTagDetailId;
        private string _playUri;

        public GetLiveAdTagDetailTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateLiveSessionSample();
            _getSample = new GetLiveAdTagDetailSample();
            _listSample = new ListLiveAdTagDetailsSample();
        }
        public async Task InitializeAsync()
        {
            var session = _createSample.CreateLiveSession(
               _fixture.ProjectId, _fixture.LocationId, _fixture.TestLiveConfigId);
            _liveSessionId = session.LiveSessionName.LiveSessionId;
            _playUri = session.PlayUri;

            await _fixture.GetManifestAndRendition(_playUri);

            var adTagDetails = _listSample.ListLiveAdTagDetails(_fixture.ProjectId, _fixture.LocationId, _liveSessionId);
            // Only one adTagDetail in list.
            _adTagDetailId = adTagDetails.First().LiveAdTagDetailName.LiveAdTagDetailId;
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void GetsLiveAdTagDetail()
        {
            // Run the sample code.
            var result = _getSample.GetLiveAdTagDetail(
                _fixture.ProjectId, _fixture.LocationId, _liveSessionId, _adTagDetailId);

            Assert.Equal(_adTagDetailId, result.LiveAdTagDetailName.LiveAdTagDetailId);
        }
    }
}
