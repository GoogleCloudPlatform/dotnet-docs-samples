// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START redis_update_instance]

using Google.Cloud.Redis.V1;
using Google.LongRunning;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System;

public class UpdateInstanceSample
{
    /// <summary>
    /// Updates the metadata and configuration of a specific Redis instance.
    /// </summary>
    public Instance UpdateInstance(string projectId, string locationId, string instanceId)
    {
        // Create client
        CloudRedisClient cloudRedisClient = CloudRedisClient.Create();
        var labelsToUpdate = new MapField<string, string>
            {
                { "environment", "development" }
            };
        UpdateInstanceRequest updateInstanceRequest = new UpdateInstanceRequest
        {
            UpdateMask = new FieldMask()
            {
                Paths = { "labels" }
            },
            Instance = new Instance()
            {
                Labels = { labelsToUpdate },
                InstanceName = new InstanceName(projectId, locationId, instanceId)
            }
        };
        // Make the UpdateInstance request
        Operation<Instance, OperationMetadata> response = cloudRedisClient.UpdateInstance(updateInstanceRequest);

        Console.WriteLine("Waiting for the instance update operation to complete.");
        // Poll until the returned long-running operation is complete
        Operation<Instance, OperationMetadata> completedResponse = response.PollUntilCompleted();

        Console.WriteLine(
                $"Instance: {instanceId} {(completedResponse.IsCompleted ? "was successfully updated in " : "failed to update in ")}{projectId} project");

        return completedResponse.Result;
    }
}
// [END redis_update_instance]
