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

using Grpc.Core;
using System.Threading.Tasks;
using Xunit;

namespace Compute.Samples.Tests
{
    [Collection(nameof(ComputeFixture))]
    public class DeleteInstanceAsyncTest
    {
        private readonly ComputeFixture _fixture;
        private readonly CreateInstanceAsyncSample _createSample = new CreateInstanceAsyncSample();
        private readonly DeleteInstanceAsyncSample _deleteSample = new DeleteInstanceAsyncSample();

        public DeleteInstanceAsyncTest(ComputeFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task DeletesInstance()
        {
            string machineName = _fixture.GenerateMachineName();
            // Let's use the create instance sample because that polls until completed.
            await _createSample.CreateInstanceAsync(_fixture.ProjectId, _fixture.Zone, machineName, _fixture.MachineType, _fixture.DiskImage, _fixture.DiskSizeGb);

            await _deleteSample.DeleteInstanceAsync(_fixture.ProjectId, _fixture.Zone, machineName);

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.InstancesClient.GetAsync(_fixture.ProjectId, _fixture.Zone, machineName));
            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }
    }
}
