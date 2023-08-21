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

namespace LiveStream.Samples.Tests
{
    [Collection(nameof(LiveStreamFixture))]
    public class ListAssetsTest
    {
        private LiveStreamFixture _fixture;
        private readonly ListAssetsSample _listSample;

        public ListAssetsTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _listSample = new ListAssetsSample();
        }

        [Fact]
        public void ListsAssets()
        {
            var assets = _listSample.ListAssets(_fixture.ProjectId, _fixture.LocationId);
            Assert.Contains(assets, r => _fixture.TestAssetId == r.AssetName.AssetId);
        }
    }
}