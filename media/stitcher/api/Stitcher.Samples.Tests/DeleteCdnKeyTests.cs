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
    public class DeleteCdnKeyTest
    {
        private StitcherFixture _fixture;
        private readonly CreateCdnKeySample _createSample;
        private readonly DeleteCdnKeySample _deleteSample;

        private string _cdnKeyId;

        public DeleteCdnKeyTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateCdnKeySample();
            _deleteSample = new DeleteCdnKeySample();
            _cdnKeyId = $"{_fixture.CloudCdnKeyIdPrefix}-{_fixture.TimestampId()}";

            _fixture.CdnKeyIds.Add(_cdnKeyId);
            _createSample.CreateCdnKey(
                _fixture.ProjectId, _fixture.LocationId,
                _cdnKeyId, _fixture.Hostname, _fixture.CloudCdnKeyName, _fixture.CloudCdnTokenKey, null);
        }

        [Fact]
        public void DeletesCdnKey() =>
            // Run the sample code.
            _deleteSample.DeleteCdnKey(_fixture.ProjectId, _fixture.LocationId, _cdnKeyId);
    }
}
