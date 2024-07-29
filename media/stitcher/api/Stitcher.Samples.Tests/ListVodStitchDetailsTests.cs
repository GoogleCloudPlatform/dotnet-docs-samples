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
    public class ListVodStitchDetailsTest
    {
        private StitcherFixture _fixture;
        private readonly CreateVodSessionSample _createSample;
        private readonly ListVodStitchDetailsSample _listSample;

        private string _vodSessionId;

        public ListVodStitchDetailsTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateVodSessionSample();
            _listSample = new ListVodStitchDetailsSample();

            var result = _createSample.CreateVodSession(
               _fixture.ProjectId, _fixture.LocationId, _fixture.TestVodConfigId);
            _vodSessionId = result.VodSessionName.VodSessionId;
        }

        [Fact]
        public void ListsVodStitchDetails()
        {
            // Run the sample code.
            var vodStitchDetails = _listSample.ListVodStitchDetails(
                _fixture.ProjectId, _fixture.LocationId, _vodSessionId);

            Assert.Contains(vodStitchDetails, r => _vodSessionId == r.VodStitchDetailName.VodSessionId);
        }
    }
}
