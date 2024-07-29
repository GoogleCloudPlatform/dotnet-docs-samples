/*
 * Copyright 2024 Google LLC
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
    public class UpdateVodConfigAsyncTest
    {
        private StitcherFixture _fixture;
        private readonly UpdateVodConfigSample _updateSample;

        public UpdateVodConfigAsyncTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _updateSample = new UpdateVodConfigSample();
        }

        [Fact]
        public async Task UpdatesVodConfigAsync()
        {
            var result = await _updateSample.UpdateVodConfigAsync(_fixture.ProjectId, _fixture.LocationId, _fixture.TestVodConfigId, _fixture.UpdateVodSourceUri);
            Assert.Equal(_fixture.TestVodConfigId, result.VodConfigName.VodConfigId);
            Assert.Equal(_fixture.UpdateVodSourceUri, result.SourceUri);
        }
    }
}
