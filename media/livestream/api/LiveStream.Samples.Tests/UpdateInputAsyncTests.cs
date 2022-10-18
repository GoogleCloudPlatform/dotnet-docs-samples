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
using System.Threading.Tasks;

namespace LiveStream.Samples.Tests
{
    [Collection(nameof(LiveStreamFixture))]
    public class UpdateInputAsyncTest
    {
        private LiveStreamFixture _fixture;
        private readonly UpdateInputSample _updateSample;

        public UpdateInputAsyncTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _updateSample = new UpdateInputSample();
        }

        [Fact]
        public async Task UpdatesInputAsync()
        {
            var result = await _updateSample.UpdateInputAsync(_fixture.ProjectId, _fixture.LocationId, _fixture.TestInputId);
            Assert.Equal(_fixture.ProjectId, result.InputName.ProjectId);
            Assert.Equal(_fixture.LocationId, result.InputName.LocationId);
            Assert.Equal(_fixture.TestInputId, result.InputName.InputId);
            Assert.Equal(_fixture.UpdateTopPixels, result.PreprocessingConfig.Crop.TopPixels);
            Assert.Equal(_fixture.UpdateBottomPixels, result.PreprocessingConfig.Crop.BottomPixels);
        }
    }
}