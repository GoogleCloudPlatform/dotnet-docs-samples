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

using System.Threading.Tasks;
using Xunit;

namespace Stitcher.Samples.Tests
{
    [Collection(nameof(StitcherFixture))]
    public class ListLiveAdTagDetailsTest : IAsyncLifetime
    {
        private StitcherFixture _fixture;
        private readonly CreateLiveSessionSample _createSample;
        private readonly ListLiveAdTagDetailsSample _listSample;

        private string _liveSessionId;
        private string _playUri;

        public ListLiveAdTagDetailsTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateLiveSessionSample();
            _listSample = new ListLiveAdTagDetailsSample();
        }

        public async Task InitializeAsync()
        {
            var result = _createSample.CreateLiveSession(
               _fixture.ProjectId, _fixture.LocationId, _fixture.TestLiveConfigId);
            _liveSessionId = result.LiveSessionName.LiveSessionId;
            _playUri = result.PlayUri;

            await _fixture.GetManifestAndRendition(_playUri);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public void ListsLiveAdTagDetails()
        {
            // Run the sample code.
            var liveAdTagDetails = _listSample.ListLiveAdTagDetails(
                _fixture.ProjectId, _fixture.LocationId, _liveSessionId);

            Assert.Contains(liveAdTagDetails, r => _liveSessionId == r.LiveAdTagDetailName.LiveSessionId);
        }
    }
}
