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

// [START compute_instances_create]
// [START compute_instances_operation_check]

using Google.Cloud.Compute.V1;
using System.Threading.Tasks;

public class CreateInstanceAsyncSample
{
    public async Task CreateInstanceAsync(
        // TODO(developer): Set your own default values for these parameters or pass different values when calling this method.
        string projectId = "your-project-id",
        string zone = "us-central1-a",
        string machineName = "test-machine",
        string machineType = "n1-standard-1",
        string diskImage = "projects/debian-cloud/global/images/family/debian-12",
        long diskSizeGb = 10,
        string networkName = "default")
    {
        Instance instance = new Instance
        {
            Name = machineName,
            // See https://cloud.google.com/compute/docs/machine-types for more information on machine types.
            MachineType = $"zones/{zone}/machineTypes/{machineType}",
            // Instance creation requires at least one persistent disk.
            Disks =
            {
                new AttachedDisk
                {
                    AutoDelete = true,
                    Boot = true,
                    Type = ComputeEnumConstants.AttachedDisk.Type.Persistent,
                    InitializeParams = new AttachedDiskInitializeParams 
                    {
                        // See https://cloud.google.com/compute/docs/images for more information on available images.
                        SourceImage = diskImage,
                        DiskSizeGb = diskSizeGb
                    }
                }
            },
            NetworkInterfaces = { new NetworkInterface { Name = networkName } }
        };

        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        InstancesClient client = await InstancesClient.CreateAsync();

        // Insert the instance in the specified project and zone.
        var instanceCreation = await client.InsertAsync(projectId, zone, instance);

        // Wait for the operation to complete using client-side polling.
        // The server-side operation is not affected by polling,
        // and might finish successfully even if polling times out.
        await instanceCreation.PollUntilCompletedAsync();
    }
}

// [END compute_instances_operation_check]
// [END compute_instances_create]
