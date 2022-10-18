// Copyright 2022 Google Inc.
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

// [START spanner_update_instance_config]

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;

public class UpdateInstanceConfigAsyncSample
{
    public async Task<Operation<InstanceConfig, UpdateInstanceConfigMetadata>> UpdateInstanceConfigAsync(string projectId, string instanceConfigId)
    {
        InstanceAdminClient instanceAdminClient = await InstanceAdminClient.CreateAsync();
        var instanceConfigName = new InstanceConfigName(projectId, instanceConfigId);

        var instanceConfig = new InstanceConfig();
        instanceConfig.InstanceConfigName  = instanceConfigName;

        instanceConfig.DisplayName = "New display name";
        instanceConfig.Labels.Add(new Dictionary<string, string>
        {
            {"cloud_spanner_samples","true"},
            {"updated","true"},
        });

        var updateInstanceConfigOperation = await instanceAdminClient.UpdateInstanceConfigAsync(new UpdateInstanceConfigRequest
        {
            InstanceConfig = instanceConfig,
            UpdateMask = new FieldMask
            {
                Paths = { "display_name", "labels" }
            }
        });

        updateInstanceConfigOperation = await updateInstanceConfigOperation.PollUntilCompletedAsync(); 

        if (updateInstanceConfigOperation.IsFaulted)
        {
            throw updateInstanceConfigOperation.Exception;
        }

        Console.WriteLine("Update Instance Config operation completed successfully.");
        return updateInstanceConfigOperation;
    }
}

// [END spanner_update_instance_config]