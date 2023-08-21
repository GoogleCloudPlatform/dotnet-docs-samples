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
    public class UpdatePoolAsyncTest
    {
        private LiveStreamFixture _fixture;
        private readonly UpdatePoolSample _updateSample;

        public UpdatePoolAsyncTest(LiveStreamFixture fixture)
        {
            _fixture = fixture;
            _updateSample = new UpdatePoolSample();
        }

        // Pool updates cannot be run with the fixture since the fixture
        // creates a channel test resource and starts it. Pool updates
        // cannot be run if tests are run in parallel.
        [Fact(Skip = "pool updates require all channels to be stopped")]
        public async Task UpdatesPoolAsync()
        {
            // Pool updates take a long time to run, so the test updates the peered
            // network with the same value to decrease this time.
            await _updateSample.UpdatePoolAsync(_fixture.ProjectId, _fixture.LocationId, _fixture.TestPoolId, "");
        }
    }
}