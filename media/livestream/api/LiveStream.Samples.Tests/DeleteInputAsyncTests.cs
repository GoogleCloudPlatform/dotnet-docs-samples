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
    public class DeleteInputAsyncTest : IAsyncLifetime
    {
        private LiveStreamFixture _fixture;
        private readonly CreateInputSample _createSample;
        private readonly DeleteInputSample _deleteSample;
        private string _inputId;

        public DeleteInputAsyncTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateInputSample();
            _deleteSample = new DeleteInputSample();
            _inputId = $"{_fixture.InputIdPrefix}-{_fixture.RandomId()}";
            _fixture.InputIds.Add(_inputId);
        }

        public async Task InitializeAsync()
        {
            await _createSample.CreateInputAsync(
                    _fixture.ProjectId, _fixture.LocationId,
                    _inputId);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public async Task DeletesInputAsync()
        {
            await _deleteSample.DeleteInputAsync(_fixture.ProjectId, _fixture.LocationId, _inputId);
        }
    }
}