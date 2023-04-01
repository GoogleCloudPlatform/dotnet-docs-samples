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
    public class UpdateCdnKeyAkamaiTest
    {
        private StitcherFixture _fixture;
        private readonly UpdateCdnKeyAkamaiSample _updateSample;

        public UpdateCdnKeyAkamaiTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _updateSample = new UpdateCdnKeyAkamaiSample();
        }

        [Fact]
        public void UpdatesCdnKey_Akamai()
        {
            var result = _updateSample.UpdateCdnKeyAkamai(_fixture.ProjectId, _fixture.LocationId, _fixture.TestAkamaiCdnKeyId, _fixture.UpdatedAkamaiHostname, _fixture.UpdatedAkamaiTokenKey);
            Assert.Equal(_fixture.TestAkamaiCdnKeyId, result.CdnKeyName.CdnKeyId);
            Assert.Equal(_fixture.UpdatedAkamaiHostname, result.Hostname);
        }
    }
}