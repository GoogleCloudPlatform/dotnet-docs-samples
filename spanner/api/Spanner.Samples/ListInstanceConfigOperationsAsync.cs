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

// [START spanner_list_instance_config_operations]

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Spanner.Admin.Instance.V1;
using Google.LongRunning;

public class ListInstanceConfigOperationsAsyncSample
{
    public async Task<IEnumerable<Operation>> ListInstanceConfigOperationsAsync(string projectId)
    {
        var instanceAdminClient = await InstanceAdminClient.CreateAsync();
        var projectName = ProjectName.FromProject(projectId);
        var listInstanceConfigOperations = instanceAdminClient.ListInstanceConfigOperations(projectName);

        // We print the first 5 elements for demonstration purposes.
        // You can print all operations in the sequence by removing the call to Take(5).
        // The sequence will lazily fetch elements in pages as needed.
        foreach (var operation in listInstanceConfigOperations.Take(5))
        {
            Console.WriteLine($"Operation {operation.Name} of type {operation.Metadata.TypeUrl} is finished {operation.Done}");
        }
        return listInstanceConfigOperations;
    }
}

// [END spanner_list_instance_config_operations]