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
    public class ListVodAdTagDetailsTest
    {
        private StitcherFixture _fixture;
        private readonly CreateVodSessionSample _createSample;
        private readonly ListVodAdTagDetailsSample _listSample;

        private string _vodSessionId;

        public ListVodAdTagDetailsTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateVodSessionSample();
            _listSample = new ListVodAdTagDetailsSample();

            var result = _createSample.CreateVodSession(
               _fixture.ProjectId, _fixture.LocationId, _fixture.TestVodConfigId);
            _vodSessionId = result.VodSessionName.VodSessionId;
        }

        [Fact]
        public void ListsVodAdTagDetails()
        {
            // Run the sample code.
            var vodAdTagDetails = _listSample.ListVodAdTagDetails(
                _fixture.ProjectId, _fixture.LocationId, _vodSessionId);

            Assert.Contains(vodAdTagDetails, r => _vodSessionId == r.VodAdTagDetailName.VodSessionId);
        }
    }
}
