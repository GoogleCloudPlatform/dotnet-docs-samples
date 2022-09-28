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

using System.Text.RegularExpressions;
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

            var session = _createSample.CreateLiveSession(
               _fixture.ProjectId, _fixture.LocationId, _fixture.LiveSourceUri, _fixture.LiveAdTagUri, _fixture.TestSlateId);
            _liveSessionId = session.LiveSessionName.LiveSessionId;
            _playUri = session.PlayUri;
        }
        public async Task InitializeAsync()
        {
            // To get ad tag details, you need to make a request to the main manifest and
            // a rendition first. This supplies media player information to the API.
            //
            // Curl the playUri first. The last line of the response will contain a
            // renditions location. Curl the live session name with the rendition
            // location appended.
            string renditions = await _fixture.GetHttpResponse(_playUri);
            Match m = Regex.Match(renditions, "renditions/.*", RegexOptions.IgnoreCase);

            // playUri will be in the following format:
            // https://videostitcher.googleapis.com/v1/projects/{project}/locations/{location}/liveSessions/{session-id}/manifest.m3u8?signature=...
            // Replace manifest.m3u8?signature=... with the renditions location.
            string tmp = _playUri.Substring(0, _playUri.LastIndexOf("/"));
            string stitchedUrl = $"{tmp}/{m.Value}";
            await _fixture.GetHttpResponse(stitchedUrl); // Curl the live session name with rendition

            var adTagDetails = _listSample.ListLiveAdTagDetails(_fixture.ProjectId, _fixture.LocationId, _liveSessionId);
            // Only one adTagDetail in list.
            foreach (Google.Cloud.Video.Stitcher.V1.LiveAdTagDetail adTagDetail in adTagDetails)
            {
                _adTagDetailId = adTagDetail.LiveAdTagDetailName.LiveAdTagDetailId;
            }
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
