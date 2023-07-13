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
    public class DeleteSlateAsyncTest : IAsyncLifetime
    {
        private StitcherFixture _fixture;
        private readonly CreateSlateSample _createSample;
        private readonly DeleteSlateSample _deleteSample;

        private string _slateId;

        public DeleteSlateAsyncTest(StitcherFixture fixture)
        {
            _fixture = fixture;
            _createSample = new CreateSlateSample();
            _deleteSample = new DeleteSlateSample();
            _slateId = $"{_fixture.SlateIdPrefix}-{_fixture.RandomId()}-{_fixture.TimestampId()}";
            _fixture.SlateIds.Add(_slateId);
        }

        public async Task InitializeAsync()
        {
            await _createSample.CreateSlateAsync(
                _fixture.ProjectId, _fixture.LocationId,
                _slateId, _fixture.TestSlateUri);
        }

        public async Task DisposeAsync()
        {
        }

        [Fact]
        public async Task DeletesSlateAsync() =>
            // Run the sample code.
            await _deleteSample.DeleteSlateAsync(_fixture.ProjectId, _fixture.LocationId, _slateId);
    }
}
