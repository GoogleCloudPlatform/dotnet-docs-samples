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

// [START compute_instances_list]

using Google.Cloud.Compute.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ListZoneInstancesAsyncSample
{
    public async Task<IList<Instance>> ListZoneInstancesAsync(
        // TODO(developer): Set your own default values for these parameters or pass different values when calling this method.
        string projectId = "your-project-id", 
        string zone = "us-central1-a")
    {
        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        InstancesClient client = await InstancesClient.CreateAsync();
        InstanceList instanceList;
        IList<Instance> allInstances = new List<Instance>();

        // Make the request to list all VM instances in the given zone in the specified project.
        ListInstancesRequest request = new ListInstancesRequest 
        { 
            Project = projectId,
            Zone = zone,
        };
        do
        {
            instanceList = await client.ListAsync(request);
            // The result is an Instance collection.
            foreach (var instance in instanceList.Items)
            {
                Console.WriteLine($"-- Name: {instance.Name}");
                allInstances.Add(instance);
            }
            // Use the NextPageToken value on the request result to make subsequent requests
            // until all instances have been listed.
            request.PageToken = instanceList.NextPageToken;

            // When all instances are listed the last result NextPageToken is not set.
        } while (instanceList.HasNextPageToken);

        return allInstances;
    }
}

// [END compute_instances_list]
