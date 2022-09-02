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
    public class CreateCdnKeyTest
    {
        private StitcherFixture _fixture;
        private readonly CreateCdnKeySample _createSample;

        private string _akamaiCdnKeyId;
        private string _cloudCdnKeyId;

        public CreateCdnKeyTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateCdnKeySample();
            _akamaiCdnKeyId = $"{_fixture.AkamaiCdnKeyIdPrefix}-{_fixture.RandomId()}";
            _cloudCdnKeyId = $"{_fixture.CloudCdnKeyIdPrefix}-{_fixture.RandomId()}";
        }

        [Fact]
        public void CreatesCdnKey()
        {
            // Run the sample code and create an Akamai CDN key.
            var result = _createSample.CreateCdnKey(
                _fixture.ProjectId, _fixture.LocationId,
                _akamaiCdnKeyId, _fixture.Hostname, "", "", _fixture.AkamaiTokenKey);
            _fixture.CdnKeyIds.Add(_akamaiCdnKeyId);

            Assert.Equal(_akamaiCdnKeyId, result.CdnKeyName.CdnKeyId);

            // Run the sample code and create a Cloud CDN key.
            result = _createSample.CreateCdnKey(
                _fixture.ProjectId, _fixture.LocationId,
                _cloudCdnKeyId, _fixture.Hostname, _fixture.CloudCdnKeyName, _fixture.CloudCdnTokenKey, "");
            _fixture.CdnKeyIds.Add(_cloudCdnKeyId);

            Assert.Equal(_cloudCdnKeyId, result.CdnKeyName.CdnKeyId);
        }
    }
}
