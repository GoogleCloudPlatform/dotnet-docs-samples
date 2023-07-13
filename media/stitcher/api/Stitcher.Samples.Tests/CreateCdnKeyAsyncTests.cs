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
    public class CreateCdnKeyAsyncTest
    {
        private StitcherFixture _fixture;
        private readonly CreateCdnKeySample _createSample;

        private string _cloudCdnKeyId;
        private string _mediaCdnKeyId;

        public CreateCdnKeyAsyncTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateCdnKeySample();
            _cloudCdnKeyId = $"{_fixture.CloudCdnKeyIdPrefix}-{_fixture.RandomId()}-{_fixture.TimestampId()}";
            _mediaCdnKeyId = $"{_fixture.MediaCdnKeyIdPrefix}-{_fixture.RandomId()}-{_fixture.TimestampId()}";
        }

        [Fact]
        public async Task CreatesCdnKeyAsync_MediaCdn()
        {
            // Run the sample code and create a Media CDN key.
            _fixture.CdnKeyIds.Add(_mediaCdnKeyId);
            var result = await _createSample.CreateCdnKeyAsync(
                _fixture.ProjectId, _fixture.LocationId,
                _mediaCdnKeyId, _fixture.Hostname, _fixture.KeyName, _fixture.MediaCdnPrivateKey, true);

            Assert.Equal(_mediaCdnKeyId, result.CdnKeyName.CdnKeyId);
        }

        [Fact]
        public async Task CreatesCdnKeyAsync_CloudCdn()
        {
            // Run the sample code and create a Cloud CDN key.
            _fixture.CdnKeyIds.Add(_cloudCdnKeyId);
            var result = await _createSample.CreateCdnKeyAsync(
                _fixture.ProjectId, _fixture.LocationId,
                _cloudCdnKeyId, _fixture.Hostname, _fixture.KeyName, _fixture.CloudCdnPrivateKey, false);

            Assert.Equal(_cloudCdnKeyId, result.CdnKeyName.CdnKeyId);
        }
    }
}
