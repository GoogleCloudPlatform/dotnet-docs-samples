// Copyright 2022 Google Inc.>
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

// [START spanner_create_instance_config]

using System;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Spanner.Admin.Instance.V1;

public class CreateInstanceConfigAsyncSample
{
    public async Task<InstanceConfig> CreateInstanceConfigAsync(string projectId, string baseInstanceConfigId, string customInstanceConfigId)
    {
        InstanceAdminClient instanceAdminClient = await InstanceAdminClient.CreateAsync();
        var instanceConfigName = new InstanceConfigName(projectId, baseInstanceConfigId);
        var instanceConfig = await instanceAdminClient.GetInstanceConfigAsync(instanceConfigName);

        var customInstanceConfigName = new InstanceConfigName(projectId, customInstanceConfigId);

        //Create a custom config.
        InstanceConfig userInstanceConfig = new InstanceConfig
        {
            DisplayName = "C# test custom instance config",
            ConfigType = InstanceConfig.Types.Type.UserManaged,
            BaseConfigAsInstanceConfigName = instanceConfigName,
            InstanceConfigName = customInstanceConfigName,
            //The replicas for the custom instance configuration must include all the replicas of the base
            //configuration, in addition to at least one from the list of optional replicas of the base
            //configuration.
            Replicas = { instanceConfig.Replicas },
            OptionalReplicas =
            {
                new ReplicaInfo
                {
                    Type = ReplicaInfo.Types.ReplicaType.ReadOnly,
                    Location = "us-east1",
                    DefaultLeaderLocation = false
                },
                //The replicas for the custom instance configuration must include all the replicas of the base
                //configuration, in addition to at least one from the list of optional replicas of the base
                //configuration.
                instanceConfig.OptionalReplicas
            }
        };

        var operationResult = await instanceAdminClient.CreateInstanceConfigAsync(new CreateInstanceConfigRequest
        {
            ParentAsProjectName = ProjectName.FromProject(projectId),
            InstanceConfig = userInstanceConfig,
            InstanceConfigId = customInstanceConfigId
        });

        var pollResult = await operationResult.PollUntilCompletedAsync();

        if (pollResult.IsFaulted)
        {
            throw pollResult.Exception;
        }

        Console.WriteLine($"Instance config created successfully");
        Console.WriteLine($"Available custom replication options for instance config {pollResult.Result.Name}\r\n{pollResult.Result.OptionalReplicas}");

        return pollResult.Result;
    }
}


// [END spanner_create_instance_config]