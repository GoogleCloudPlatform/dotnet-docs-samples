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

// [START redis_get_instance]

using Google.Cloud.Redis.V1;
using System;

public class GetInstanceSample
{
    /// <summary>
    /// Gets the details of a specific Redis instance.
    /// </summary>
    public Instance GetInstance(string projectId = "my-project",
        string locationId = "us-east1", string instanceId = "my-instance")
    {
        // Create client
        CloudRedisClient cloudRedisClient = CloudRedisClient.Create();
        InstanceName instanceName = new InstanceName(projectId, locationId, instanceId);

        // Make the GetInstance request
        Instance instance = cloudRedisClient.GetInstance(instanceName);

        Console.WriteLine($"InstanceId:\t{instance.InstanceName.InstanceId}");
        Console.WriteLine($"State:\t{instance.State}");
        Console.WriteLine($"Tier:\t{instance.Tier}");
        Console.WriteLine($"Host:\t{instance.Host}");
        Console.WriteLine($"Location:\t{instance.LocationId}");
        Console.WriteLine($"Memory Size(GB):\t{instance.MemorySizeGb}");
        Console.WriteLine($"Version:\t{instance.RedisVersion}");
        Console.WriteLine($"Reserved IP Range:\t{instance.ReservedIpRange}");

        return instance;
    }
}
// [END redis_get_instance]
