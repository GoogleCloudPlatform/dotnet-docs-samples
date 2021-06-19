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

using Google.Cloud.Compute.V1;
using System.Threading.Tasks;
using Xunit;

namespace Compute.Samples.Tests
{
    [Collection(nameof(ComputeFixture))]
    public class OperationPollerTest
    {
        private readonly ComputeFixture _fixture;

        public OperationPollerTest(ComputeFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task PollsZoneOperationUntilCompleted()
        {
            string machineName = _fixture.GenerateMachineName();

            Instance instance = new Instance
            {
                Name = machineName,
                MachineType = $"zones/{_fixture.Zone}/machineTypes/{_fixture.MachineType}",
                Disks =
            {
                new AttachedDisk
                {
                    AutoDelete = true,
                    Boot = true,
                    Type = AttachedDisk.Types.Type.Persistent,
                    InitializeParams = new AttachedDiskInitializeParams
                    {
                        SourceImage = _fixture.DiskImage,
                        DiskSizeGb = _fixture.DiskSizeGb
                    }
                }
            },
                NetworkInterfaces = { new NetworkInterface { Name = "default" } }
            };

            Operation operation = await _fixture.InstancesClient.InsertAsync(_fixture.ProjectId, _fixture.Zone, instance);
            operation = await operation.PollUntilCompletedAsync(_fixture.ProjectId, _fixture.Zone);

            Assert.Equal(Operation.Types.Status.Done, operation.Status);
        }

        [Fact]
        public async Task PollsGlobalOperationUntilCompleted()
        {
            var project = await _fixture.ProjectsClient.GetAsync(_fixture.ProjectId);
            var usageExportLocation = project.UsageExportLocation;

            var noChangeRequest = new SetUsageExportBucketProjectRequest
            {
                Project = _fixture.ProjectId,
                UsageExportLocationResource = usageExportLocation ?? new UsageExportLocation(),
            };

            var operation = await _fixture.ProjectsClient.SetUsageExportBucketAsync(noChangeRequest);

            operation = await operation.PollUntilCompletedAsync(_fixture.ProjectId);

            Assert.Equal(Operation.Types.Status.Done, operation.Status);
        }
    }
}
