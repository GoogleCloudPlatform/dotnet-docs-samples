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
    public class ListSlatesTest
    {
        private StitcherFixture _fixture;
        private readonly ListSlatesSample _listSample;

        public ListSlatesTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _listSample = new ListSlatesSample();
        }

        [Fact]
        public void ListsSlates()
        {
            var slates = _listSample.ListSlates(_fixture.ProjectId, _fixture.LocationId);
            Assert.Contains(slates, r => _fixture.TestSlateId == r.SlateName.SlateId);
        }
    }
}