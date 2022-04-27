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

// [START spanner_delete_instance_config]

using System;
using System.Threading.Tasks;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Grpc.Core;

public class DeleteInstanceConfigAsyncSample
{

    public async Task DeleteInstanceConfigAsync(string projectId, string instanceConfigId)
    {
        InstanceAdminClient instanceAdminClient = await InstanceAdminClient.CreateAsync();
        var instanceConfigName = new InstanceConfigName(projectId, instanceConfigId);

        try
        {
            await instanceAdminClient.DeleteInstanceConfigAsync(new DeleteInstanceConfigRequest
            {
                InstanceConfigName = instanceConfigName
            });
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.NotFound)
        {
            Console.WriteLine("The specified instance config does not exist. It cannot be deleted.");
            return;
        }

        Console.WriteLine("Delete Instance Config operation is completed");
    }
}


// [END spanner_delete_instance_config]