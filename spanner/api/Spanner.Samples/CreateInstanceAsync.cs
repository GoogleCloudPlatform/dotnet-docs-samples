// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START spanner_create_instance]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using System;
using System.Threading.Tasks;

public class CreateInstanceAsyncSample
{
    public async Task<Instance> CreateInstanceAsync(
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
            ConfigAsInstanceConfigName = InstanceConfigName.FromProjectInstanceConfig(projectId, "regional-us-central1"),
            DisplayName = "This is a display name.",
            NodeCount = 1,
            Labels =
            {
                { "cloud_spanner_samples", "true" },
            },
            Edition = edition,
        };
        ProjectName projectName = ProjectName.FromProject(projectId);

        // Make the CreateInstance request.
        Operation<Instance, CreateInstanceMetadata> response = await instanceAdminClient.CreateInstanceAsync(projectName, instanceId, instance);

        Console.WriteLine("Waiting for the operation to finish.");

        // Poll until the returned long-running operation is complete.
        Operation<Instance, CreateInstanceMetadata> completedResponse = await response.PollUntilCompletedAsync();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating instance: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Console.WriteLine($"Instance created successfully.");

        return completedResponse.Result;
    }
}
// [END spanner_create_instance]
