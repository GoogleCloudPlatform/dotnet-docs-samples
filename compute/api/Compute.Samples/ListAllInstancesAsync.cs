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
    public async Task<IList<Instance>> ListAllInstancesAsync(string projectId = "your-project-id")
    {
        // Initialize the client that will be used to send instance-related requests.
        // You should reuse the same client for multiple requests.
        InstancesClient client = await InstancesClient.CreateAsync();
        InstanceAggregatedList instanceList;
        IList<Instance> allInstances = new List<Instance>();

        // Make the requests to list all VM instances.
        AggregatedListInstancesRequest request = new AggregatedListInstancesRequest { Project = projectId };
        do
        {
            instanceList = await client.AggregatedListAsync(request);
            // The result contains a KeyValuePair collection, where the key is a zone and the value
            // is a collection of instances in that zone.
            foreach (var instancesByZone in instanceList.Items)
            {
                Console.WriteLine($"Instances for zone: {instancesByZone.Key}");
                foreach (var instance in instancesByZone.Value.Instances)
                {
                    Console.WriteLine($"-- Name: {instance.Name}");
                    allInstances.Add(instance);
                }
            }
            // Use the NextPageToken value on the request result to make subsequent requests
            // until all instances have been listed.
            request.PageToken = instanceList.NextPageToken;

        // When all instances are listed the last result NextPageToken is not set.
        } while (instanceList.HasNextPageToken);

        return allInstances;
    }
}

// [END compute_instances_list_all]
