﻿// Copyright 2021 Google Inc.
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

// [START spanner_get_instance_config]

using Google.Cloud.Spanner.Admin.Instance.V1;
using System;
using System.Threading.Tasks;

public class GetInstanceConfigAsyncSample
{
    public async Task<InstanceConfig> GetInstanceConfigAsync(string projectId, string instanceConfigId)
    {
        var instanceAdminClient = await InstanceAdminClient.CreateAsync();
        var instanceConfigName = InstanceConfigName.FromProjectInstanceConfig(projectId, instanceConfigId);
        var instanceConfig = await instanceAdminClient.GetInstanceConfigAsync(instanceConfigName);

        Console.WriteLine($"Available leader options for instance config {instanceConfigName.InstanceConfigId}:");
        foreach (var leader in instanceConfig.LeaderOptions)
        {
            Console.WriteLine(leader);
        }
        return instanceConfig;
    }
}
// [END spanner_get_instance_config]
