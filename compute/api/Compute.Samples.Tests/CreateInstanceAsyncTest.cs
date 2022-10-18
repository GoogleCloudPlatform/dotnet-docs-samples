/*
 * Copyright 2021 Google LLC
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

namespace Compute.Samples.Tests
{
    [Collection(nameof(ComputeFixture))]
    public class CreateInstanceAsyncTest
    {
        private readonly ComputeFixture _fixture;
        private readonly CreateInstanceAsyncSample _sample = new CreateInstanceAsyncSample();

        public CreateInstanceAsyncTest(ComputeFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task CreatesInstance()
        {
            string machineName = _fixture.GenerateMachineName();

            await _sample.CreateInstanceAsync(
                _fixture.ProjectId, _fixture.Zone, machineName, _fixture.MachineType, _fixture.DiskImage, _fixture.DiskSizeGb, _fixture.NetworkName);

            Assert.NotNull(await _fixture.InstancesClient.GetAsync(_fixture.ProjectId, _fixture.Zone, machineName));
        }
    }
}
