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

using Xunit;
using System.Threading.Tasks;

namespace LiveStream.Samples.Tests
{
    [Collection(nameof(LiveStreamFixture))]
    public class CreateAssetAsyncTest
    {
        private LiveStreamFixture _fixture;
        private readonly CreateAssetSample _createSample;
        private string _assetId;

        public CreateAssetAsyncTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateAssetSample();
            _assetId = $"{_fixture.AssetIdPrefix}-{_fixture.RandomId()}";
            _fixture.AssetIds.Add(_assetId);
        }

        [Fact]
        public async Task CreatesAssetAsync()
        {
            var result = await _createSample.CreateAssetAsync(
                _fixture.ProjectId, _fixture.LocationId, _assetId, _fixture.AssetUri);

            Assert.Equal(_fixture.ProjectId, result.AssetName.ProjectId);
            Assert.Equal(_fixture.LocationId, result.AssetName.LocationId);
            Assert.Equal(_assetId, result.AssetName.AssetId);
        }
    }
}