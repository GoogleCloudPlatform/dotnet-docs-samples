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

namespace LiveStream.Samples.Tests
{
    [Collection(nameof(LiveStreamFixture))]
    public class GetInputTest
    {
        private LiveStreamFixture _fixture;
        private readonly GetInputSample _getSample;

        public GetInputTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _getSample = new GetInputSample();
        }

        [Fact]
        public void GetsInput()
        {
            var result = _getSample.GetInput(
                _fixture.ProjectId, _fixture.LocationId, _fixture.TestInputId);
            Assert.Equal(_fixture.ProjectId, result.InputName.ProjectId);
            Assert.Equal(_fixture.LocationId, result.InputName.LocationId);
            Assert.Equal(_fixture.TestInputId, result.InputName.InputId);
        }
    }
}