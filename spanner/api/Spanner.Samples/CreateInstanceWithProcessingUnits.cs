// Copyright 2021 Google Inc.
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

// [START spanner_create_instance_with_processing_units]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

public class CreateInstanceWithProcessingUnitsSample
{
    public Instance CreateInstanceWithProcessingUnits(string projectId, string instanceId)
    {
        // Create the InstanceAdminClient instance.
        InstanceAdminClient instanceAdminClient = InstanceAdminClient.Create();

        InstanceName instanceName = InstanceName.FromProjectInstance(projectId, instanceId);

        // Initialize request parameters.
        Instance instance = new Instance
        {
            InstanceName = instanceName,
            ConfigAsInstanceConfigName = InstanceConfigName.FromProjectInstanceConfig(projectId, "regional-us-central1"),
            DisplayName = "This is a display name.",
            ProcessingUnits = 500,
            Labels =
            {
                { "cloud_spanner_samples", "true" },
            }
        };
        ProjectName projectName = ProjectName.FromProject(projectId);

        // Make the CreateInstance request.
        Operation<Instance, CreateInstanceMetadata> response = instanceAdminClient.CreateInstance(projectName, instanceId, instance);

        Console.WriteLine("Waiting for the operation to finish.");

        // Poll until the returned long-running operation is complete.
        Operation<Instance, CreateInstanceMetadata> completedResponse = response.PollUntilCompleted();

        if (completedResponse.IsFaulted)
        {
            Console.WriteLine($"Error while creating instance: {completedResponse.Exception}");
            throw completedResponse.Exception;
        }

        Console.WriteLine($"Instance created successfully.");

        // GetInstance to get the processing units using a field mask.
        GetInstanceRequest getInstanceRequest = new GetInstanceRequest
        {
            FieldMask = new FieldMask
            {
                Paths = { "processing_units" }
            },
            InstanceName = instanceName
        };
        instance = instanceAdminClient.GetInstance(getInstanceRequest);
        Console.WriteLine($"Instance {instanceId} has {instance.ProcessingUnits} processing units.");
        return instance;
    }
}
// [END spanner_create_instance_with_processing_units]
