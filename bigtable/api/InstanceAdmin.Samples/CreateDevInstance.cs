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

// [START bigtable_create_dev_instance]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Bigtable.Admin.V2;
using Google.LongRunning;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class CreateDevInstanceSample
{
    public Instance CreateDevInstance(string projectId, string instanceId, string displayName)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        // Creates a DEVELOPMENT Instance with "<intanceId>-dev" instance ID,
        // with cluster ID "hdd-cluster" and location us-east1-b.
        // Cluster node count should not be set while creating DEVELOPMENT instance.
        displayName += " Dev"; // display name is for display purposes only, it doesn't have to equal to instanceId and can be amended after instance is created.

        // Please refer to the link below for the full list of available locations:
        // https://cloud.google.com/bigtable/docs/locations
        string zone = "us-east1-b";

        // The instance to create.
        Instance myInstance = new Instance
        {
            DisplayName = displayName,
            // You can choose DEVELOPMENT or PRODUCTION type here.
            // If not set, will default to PRODUCTION type.
            // Instance type can be upgraded from DEVELOPMENT to PRODUCTION but cannot be dowgraded after the instance is created.
            Type = Instance.Types.Type.Development,
            Labels = { { "dev-label", "dev-label" } }
        };

        // The first cluster to be created within the instance.
        Cluster myCluster = new Cluster
        {
            // You can choose SSD or HDD storage type here: StorageType.Ssd or StorageType.Hdd.
            // Cluster storage type cannot be changed after an instance is created.
            // If not set will default to SSD type.
            DefaultStorageType = StorageType.Hdd,
            LocationAsLocationName = LocationName.FromProjectLocation(projectId, zone),
        };

        // Initialize request argument(s).
        CreateInstanceRequest request = new CreateInstanceRequest
        {
            ParentAsProjectName = ProjectName.FromProject(projectId),
            Instance = myInstance,
            InstanceId = instanceId,
            // Must specify at lease one cluster.
            // Only PRODUCTION type instance can be created with more than one cluster.
            Clusters = { { "hdd-cluster", myCluster } }
        };

        Operation<Instance, CreateInstanceMetadata> createInstanceResponse =
            bigtableInstanceAdminClient.CreateInstance(request);
        Console.WriteLine("Waiting for operation to complete...");

        // Poll until the returned long-running operation is complete
        Operation<Instance, CreateInstanceMetadata> completedResponse =
            createInstanceResponse.PollUntilCompleted();

        return completedResponse.Result;
    }
}
// [END bigtable_create_dev_instance]
