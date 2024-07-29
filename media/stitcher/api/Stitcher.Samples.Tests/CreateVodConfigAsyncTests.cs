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
    public class CreateVodConfigAsyncTest
    {
        private StitcherFixture _fixture;
        private readonly CreateVodConfigSample _createSample;

        private string _vodConfigId;

        public CreateVodConfigAsyncTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateVodConfigSample();
            _vodConfigId = $"{_fixture.VodConfigIdPrefix}-{_fixture.RandomId()}-{_fixture.TimestampId()}";
        }

        [Fact]
        public async Task CreatesVodConfigAsync()
        {
            // Run the sample code.
            _fixture.VodConfigIds.Add(_vodConfigId);
            var result = await _createSample.CreateVodConfigAsync(
                _fixture.ProjectId, _fixture.LocationId, _vodConfigId, _fixture.VodSourceUri, _fixture.VodAdTagUri);

            Assert.Equal(_vodConfigId, result.VodConfigName.VodConfigId);
        }
    }
}
