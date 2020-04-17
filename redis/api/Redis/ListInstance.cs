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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Redis.V1;
using System;
using System.Linq;

namespace GoogleCloudSamples
{
    public class ListInstance
    {
        /// <summary>
        /// Lists all Redis instances owned by a project in with the specified
        /// location (region).
        /// </summary>
        public static void List(string projectId, string locationId)
        {
            // Create client
            CloudRedisClient cloudRedisClient = CloudRedisClient.Create();
            ListInstancesRequest listInstancesRequest = new ListInstancesRequest
            {
                ParentAsLocationName = LocationName.FromProjectLocation(projectId, locationId),
            };
            // Make the ListInstances request
            var instances = cloudRedisClient.ListInstances(listInstancesRequest).ToList();

            Console.WriteLine($"{"Instance Count:",-30}{instances.Count} instances in project {projectId}");

            foreach (Instance instance in instances)
            {
                Redis.PrintInstanceInfo(instance);
            }
        }
    }
}
