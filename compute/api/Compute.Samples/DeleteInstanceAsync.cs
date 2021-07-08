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

// [START compute_instances_delete]

using Google.Cloud.Compute.V1;
using System;
using System.Threading.Tasks;

public class DeleteInstanceAsyncSample
{
    public async Task DeleteInstanceAsync(
        // TODO(developer): Replace these variables before running the sample.
        string projectId = "your-project-id",
        string zone = "us-central1-a",
        string machineName = "test-machine")
    {

        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        InstancesClient client = await InstancesClient.CreateAsync();

        // Make the request to delete a VM instance.
        Operation instanceDeletion = await client.DeleteAsync(projectId, zone, machineName);

        // You may poll the operation until it completes or fails, or for a given amount of time.
        // If polling times out, the operation may still finish successfully after.
        await instanceDeletion.PollUntilCompletedAsync(projectId, zone);
    }
}

// [END compute_instances_delete]
