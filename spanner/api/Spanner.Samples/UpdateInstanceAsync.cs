// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START spanner_update_instance]

using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;

public class UpdateInstanceAsyncSample
{
    public async Task<Instance> UpdateInstanceAsync(
       string projectId,
       string instanceId,
       Instance.Types.Edition edition = Instance.Types.Edition.Standard)
    {
        // Create the InstanceAdminClient instance.
        InstanceAdminClient instanceAdminClient = await InstanceAdminClient.CreateAsync();

        // Initialize request parameters.
        Instance instance = new Instance
        {
            InstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
            Edition = edition,
        };
        FieldMask mask = new FieldMask
        {
            Paths = { "edition" }
        };

        // Make the CreateInstance request.
        Operation<Instance, UpdateInstanceMetadata> response = await instanceAdminClient.UpdateInstanceAsync(instance, mask);

        Console.WriteLine("Waiting for the operation to finish.");

        // Poll until the returned long-running operation is complete.
        Operation<Instance, UpdateInstanceMetadata> completedResponse = await response.PollUntilCompletedAsync();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while updating instance: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Console.WriteLine($"Instance updated successfully.");

        return completedResponse.Result;
    }
}

// [END spanner_update_instance]

