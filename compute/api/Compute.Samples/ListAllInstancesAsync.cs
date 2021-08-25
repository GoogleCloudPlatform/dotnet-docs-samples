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

// [START compute_instances_list_all]

using Google.Cloud.Compute.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ListAllInstancesAsyncSample
{
    public async Task<IList<Instance>> ListAllInstancesAsync(
        // TODO(developer): Set your own default values for these parameters or pass different values when calling this method.
        string projectId = "your-project-id") 
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        InstancesClient client = await InstancesClient.CreateAsync();
        IList<Instance> allInstances = new List<Instance>();

        // Make the request to list all VM instances in a project.
        await foreach (var instancesByZone in client.AggregatedListAsync(projectId))
        {
            // The result contains a KeyValuePair collection, where the key is a zone and the value
            // is a collection of instances in that zone.
            Console.WriteLine($"Instances for zone: {instancesByZone.Key}");
            foreach (var instance in instancesByZone.Value.Instances)
            {
                Console.WriteLine($"-- Name: {instance.Name}");
                allInstances.Add(instance);
            }
        }

        return allInstances;
    }
}

// [END compute_instances_list_all]
