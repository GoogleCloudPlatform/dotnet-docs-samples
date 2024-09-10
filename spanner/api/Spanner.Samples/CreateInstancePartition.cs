// Copyright 2024 Google Inc.
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

// [START spanner_create_instance_partition]

using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using System;

public class CreateInstancePartitionSample
{
    public InstancePartition CreateInstancePartition(string projectId, string instanceId, string instancePartitionId)
    {
        // Create the InstanceAdminClient instance.
        InstanceAdminClient instanceAdminClient = InstanceAdminClient.Create();

        // Initialize request parameters.
        InstancePartition partition = new InstancePartition
        {
            DisplayName = "This is a display name.",
            NodeCount = 1,
            ConfigAsInstanceConfigName = InstanceConfigName.FromProjectInstanceConfig(projectId, "nam3"),
        };
        InstanceName instanceName = InstanceName.FromProjectInstance(projectId, instanceId);

        // Make the CreateInstancePartition request.
        Operation<InstancePartition, CreateInstancePartitionMetadata> response = instanceAdminClient.CreateInstancePartition(instanceName, partition, instancePartitionId);

        Console.WriteLine("Waiting for the operation to finish.");

        // Poll until the returned long-running operation is complete.
        Operation<InstancePartition, CreateInstancePartitionMetadata> completedResponse = response.PollUntilCompleted();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating instance partition: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Console.WriteLine($"Instance created successfully.");

        return completedResponse.Result;
    }
}
// [END spanner_create_instance_partition]
