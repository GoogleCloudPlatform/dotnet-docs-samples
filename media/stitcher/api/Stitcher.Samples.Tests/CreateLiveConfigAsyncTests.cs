/*
 * Copyright 2023 Google LLC
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
    public class CreateLiveConfigAsyncTest
    {
        private StitcherFixture _fixture;
        private readonly CreateLiveConfigSample _createSample;

        private string _liveConfigId;

        public CreateLiveConfigAsyncTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateLiveConfigSample();
            _liveConfigId = $"{_fixture.LiveConfigIdPrefix}-{_fixture.RandomId()}-{_fixture.TimestampId()}";
        }

        [Fact]
        public async Task CreatesLiveConfigAsync()
        {
            // Run the sample code.
            _fixture.LiveConfigIds.Add(_liveConfigId);
            var result = await _createSample.CreateLiveConfigAsync(
                _fixture.ProjectId, _fixture.LocationId, _liveConfigId, _fixture.LiveSourceUri, _fixture.LiveAdTagUri, _fixture.TestSlateForLiveConfigId);

            Assert.Equal(_liveConfigId, result.LiveConfigName.LiveConfigId);
        }
    }
}
