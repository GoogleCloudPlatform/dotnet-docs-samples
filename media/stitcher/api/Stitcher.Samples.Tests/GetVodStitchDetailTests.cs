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
    public class GetVodStitchDetailTest
    {
        private StitcherFixture _fixture;
        private readonly CreateVodSessionSample _createSample;
        private readonly GetVodStitchDetailSample _getSample;
        private readonly ListVodStitchDetailsSample _listSample;

        private string _vodSessionId;
        private string _stitchDetailId;

        public GetVodStitchDetailTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateVodSessionSample();
            _getSample = new GetVodStitchDetailSample();
            _listSample = new ListVodStitchDetailsSample();

            var session = _createSample.CreateVodSession(
               _fixture.ProjectId, _fixture.LocationId, _fixture.TestVodConfigId);
            _vodSessionId = session.VodSessionName.VodSessionId;

            var stitchDetails = _listSample.ListVodStitchDetails(_fixture.ProjectId, _fixture.LocationId, _vodSessionId);
            // Only one stitchDetail in list.
            foreach (Google.Cloud.Video.Stitcher.V1.VodStitchDetail stitchDetail in stitchDetails)
            {
                _stitchDetailId = stitchDetail.VodStitchDetailName.VodStitchDetailId;
            }
        }

        [Fact]
        public void GetsVodStitchDetail()
        {
            // Run the sample code.
            var result = _getSample.GetVodStitchDetail(
                _fixture.ProjectId, _fixture.LocationId, _vodSessionId, _stitchDetailId);

            Assert.Equal(_stitchDetailId, result.VodStitchDetailName.VodStitchDetailId);
        }
    }
}
