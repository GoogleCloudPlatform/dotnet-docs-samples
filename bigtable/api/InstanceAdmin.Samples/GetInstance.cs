// Copyright (c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START bigtable_get_instance]

using Google.Cloud.Bigtable.Admin.V2;

public class GetInstanceSample
{
    public Instance GetInstance(string projectId, string instanceId)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        InstanceName instanceName = InstanceName.FromProjectInstance(projectId, instanceId);
        Instance instance = bigtableInstanceAdminClient.GetInstance(instanceName);

        return instance;
    }
}
// [END bigtable_get_instance]
