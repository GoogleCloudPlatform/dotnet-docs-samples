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
    public class CreateCdnKeyAkamaiAsyncTest
    {
        private StitcherFixture _fixture;
        private readonly CreateCdnKeyAkamaiSample _createSample;

        private string _akamaiCdnKeyId;

        public CreateCdnKeyAkamaiAsyncTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateCdnKeyAkamaiSample();
            _akamaiCdnKeyId = $"{_fixture.AkamaiCdnKeyIdPrefix}-{_fixture.RandomId()}-{_fixture.TimestampId()}";
        }

        [Fact]
        public async Task CreatesCdnKeyAsync_Akamai()
        {
            // Run the sample code and create an Akamai CDN key.
            _fixture.CdnKeyIds.Add(_akamaiCdnKeyId);
            var result = await _createSample.CreateCdnKeyAkamaiAsync(
                _fixture.ProjectId, _fixture.LocationId,
                _akamaiCdnKeyId, _fixture.Hostname, _fixture.AkamaiTokenKey);

            Assert.Equal(_akamaiCdnKeyId, result.CdnKeyName.CdnKeyId);
        }
    }
}
