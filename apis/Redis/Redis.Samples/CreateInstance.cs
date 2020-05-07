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

// [START redis_create_instance]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Redis.V1;
using Google.LongRunning;
using System;

public class CreateInstanceSample
{
    /// <summary>
    /// Creates a Redis instance based on the specified location.
    /// </summary>
    public Instance CreateInstance(string projectId = "my-project",
        string locationId = "us-east1", string instanceId = "my-instance")
    {
        // Create client
        CloudRedisClient cloudRedisClient = CloudRedisClient.Create();
        CreateInstanceRequest createInstanceRequest = new CreateInstanceRequest
        {
            ParentAsLocationName = LocationName.FromProjectLocation(projectId, locationId),
            InstanceId = instanceId,
            Instance = new Instance()
            {
                Tier = Instance.Types.Tier.Basic,
                MemorySizeGb = 1,
            },
        };

        // Make the CreateInstance request
        Operation<Instance, OperationMetadata> response = cloudRedisClient.CreateInstance(createInstanceRequest);

        Console.WriteLine("Waiting for the instance creation operation to complete.");
        // Poll until the returned long-running operation is complete
        Operation<Instance, OperationMetadata> completedResponse = response.PollUntilCompleted();

        Console.WriteLine(
                $"Instance: {instanceId} {(completedResponse.IsCompleted ? "was successfully created in " : "failed to create in ")}{projectId} project");

        return completedResponse.Result;
    }
}
// [END redis_create_instance]
