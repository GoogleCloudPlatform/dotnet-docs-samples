/*
 * Copyright 2021 Google LLC
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
    public class GetSlateTest
    {
        private StitcherFixture _fixture;
        private readonly GetSlateSample _getSample;

        public GetSlateTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _getSample = new GetSlateSample();
        }

        [Fact]
        public void GetsSlate()
        {
            var result = _getSample.GetSlate(_fixture.ProjectId, _fixture.LocationId, _fixture.TestSlateId);
            Assert.Equal(_fixture.TestSlateId, result.SlateName.SlateId);
        }
    }
}
