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

using Google.Cloud.Compute.V1;
using System;
using System.Threading.Tasks;

public class CreateInstanceAsyncSample
{
    public async Task CreateInstanceAsync(
        string projectId = "your-project-id",
        string zone = "us-central1-a",
        string machineName = "test-machine",
        string machineType = "n1-standard-1",
        string diskImage = "projects/debian-cloud/global/images/family/debian-10",
        string diskSizeGb = "10")
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
                    Type = AttachedDisk.Types.Type.Persistent,
                    InitializeParams = new AttachedDiskInitializeParams 
                    {
                        // See https://cloud.google.com/compute/docs/images for more information on available images.
                        SourceImage = diskImage,
                        DiskSizeGb = diskSizeGb
                    }
                }
            },
            // Instance creation requires at least one network interface.
            // The "default" network interface is created automatically for every project.
            NetworkInterfaces = { new NetworkInterface { Name = "default" } }
        };

        // Initialize the client that will be used to send instance-related requests.
        // You should reuse the same client for multiple requests.
        InstancesClient client = await InstancesClient.CreateAsync();

        // Make the request to create a VM instance.
        Operation instanceCreation = await client.InsertAsync(projectId, zone, instance);

        // You may poll the operation until it completes or fails, or for a given amount of time.
        // If polling times out, the operation may still finish successfully after.
        await instanceCreation.PollUntilCompletedAsync(projectId, zone);
    }
}

// [END compute_instances_create]
