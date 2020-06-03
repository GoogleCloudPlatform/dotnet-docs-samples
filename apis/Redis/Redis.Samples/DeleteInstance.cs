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

// [START redis_delete_instance]

using Google.Cloud.Redis.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

public class DeleteInstanceSample
{
    /// <summary>
    /// Deletes a specific Redis instance. Instance stops serving and data is
    /// deleted.
    /// </summary>
    public void DeleteInstance(string projectId = "my-project",
        string locationId = "us-east1", string instanceId = "my-instance")
    {
        // Create client
        CloudRedisClient cloudRedisClient = CloudRedisClient.Create();
        InstanceName instanceName = new InstanceName(projectId, locationId, instanceId);

        // Make the DeleteInstance request
        Operation<Empty, OperationMetadata> response = cloudRedisClient
            .DeleteInstance(instanceName);

        Console.WriteLine("Waiting for the instance delete operation to complete.");
        // Poll until the returned long-running operation is complete
        Operation<Empty, OperationMetadata> completedResponse = response.PollUntilCompleted();

        Console.WriteLine(
                $"Instance: {instanceId} {(completedResponse.IsCompleted ? "was successfully deleted in " : "failed to delete in ")}{projectId} project");
    }
}
// [END redis_delete_instance]
