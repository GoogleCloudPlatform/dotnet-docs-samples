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

// [START spanner_list_instance_configs]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Spanner.Admin.Instance.V1;
using System;
using System.Collections.Generic;
using System.Linq;

public class ListInstanceConfigsSample
{
    public IEnumerable<InstanceConfig> ListInstanceConfigs(string projectId)
    {
        var instanceAdminClient = InstanceAdminClient.Create();
        var projectName = ProjectName.FromProject(projectId);
        var instanceConfigs = instanceAdminClient.ListInstanceConfigs(projectName);

        // We print the first 5 elements for demonstration purposes.
        // You can print all configs in the sequence by removing the call to Take(5).
        // The sequence will lazily fetch elements in pages as needed.
        foreach (var instanceConfig in instanceConfigs.Take(5))
        {
            Console.WriteLine($"Available leader options for instance config {instanceConfig.InstanceConfigName.InstanceConfigId}:");
            foreach (var leader in instanceConfig.LeaderOptions)
            {
                Console.WriteLine(leader);
            }

            Console.WriteLine($"Available optional replica for instance config {instanceConfig.InstanceConfigName.InstanceConfigId}:");
            foreach (var optionalReplica in instanceConfig.OptionalReplicas)
            {
                Console.WriteLine($"Replica type - {optionalReplica.Type}, default leader location - {optionalReplica.DefaultLeaderLocation}, location - {optionalReplica.Location}");
            }
        }
        return instanceConfigs;
    }
}
// [END spanner_list_instance_configs]
