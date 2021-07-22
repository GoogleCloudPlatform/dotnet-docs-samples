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
    public class ListZoneInstancesAsyncTest
    {
        private readonly ComputeFixture _fixture;
        private readonly CreateInstanceAsyncSample _createSample = new CreateInstanceAsyncSample();
        private readonly ListZoneInstancesAsyncSample _listSample = new ListZoneInstancesAsyncSample();

        public ListZoneInstancesAsyncTest(ComputeFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task ListsInstance()
        {
            string machineName = _fixture.GenerateMachineName();
            // Let's use the create instance sample because that polls until completed.
            await _createSample.CreateInstanceAsync(_fixture.ProjectId, _fixture.Zone, machineName, _fixture.MachineType, _fixture.DiskImage, _fixture.DiskSizeGb);

            var instances = await _listSample.ListZoneInstancesAsync(_fixture.ProjectId, _fixture.Zone);

            Assert.Contains(instances, i => machineName == i.Name);
        }
    }
}
