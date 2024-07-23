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
    public class GetVodAdTagDetailTest
    {
        private StitcherFixture _fixture;
        private readonly CreateVodSessionSample _createSample;
        private readonly GetVodAdTagDetailSample _getSample;
        private readonly ListVodAdTagDetailsSample _listSample;

        private string _vodSessionId;
        private string _adTagDetailId;

        public GetVodAdTagDetailTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateVodSessionSample();
            _getSample = new GetVodAdTagDetailSample();
            _listSample = new ListVodAdTagDetailsSample();

            var session = _createSample.CreateVodSession(
               _fixture.ProjectId, _fixture.LocationId, _fixture.TestVodConfigId);
            _vodSessionId = session.VodSessionName.VodSessionId;

            var adTagDetails = _listSample.ListVodAdTagDetails(_fixture.ProjectId, _fixture.LocationId, _vodSessionId);
            // Only one adTagDetail in list.
            foreach (Google.Cloud.Video.Stitcher.V1.VodAdTagDetail adTagDetail in adTagDetails)
            {
                _adTagDetailId = adTagDetail.VodAdTagDetailName.VodAdTagDetailId;
            }
        }

        [Fact]
        public void GetsVodAdTagDetail()
        {
            // Run the sample code.
            var result = _getSample.GetVodAdTagDetail(
                _fixture.ProjectId, _fixture.LocationId, _vodSessionId, _adTagDetailId);

            Assert.Equal(_adTagDetailId, result.VodAdTagDetailName.VodAdTagDetailId);
        }
    }
}
