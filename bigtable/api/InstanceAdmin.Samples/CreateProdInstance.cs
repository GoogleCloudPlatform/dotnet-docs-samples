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

// [START bigtable_create_prod_instance]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Bigtable.Admin.V2;
using Google.LongRunning;
using System;

public class CreateProdInstanceSample
{
    public Instance CreateProdInstance(string projectId, string instanceId, string displayName)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        // Creates a production instance with "<intanceId>-prod" instance ID,
        // with cluster ID "ssd-cluster1", 3 nodes and location us-east1-b.
        displayName += " Prod"; // Display name is for display purposes only. It doesn't have to equal instanceId and can be amended after instance is created.

        // Please refer to the link below for the full list of available locations:
        // https://cloud.google.com/bigtable/docs/locations
        string zone1 = "us-east1-b";

        // The instance to create.
        Instance myInstance = new Instance
        {
            DisplayName = displayName,
            // You can choose DEVELOPMENT or PRODUCTION type here.
            // If not set, will default to PRODUCTION type.
            // Instance type can be upgraded from DEVELOPMENT to PRODUCTION but cannot be dowgraded after the instance is created.
            Type = Instance.Types.Type.Production,
            Labels = { { "prod-label", "prod-label" } }
        };

        // The first cluster to be created within the instance.
        Cluster myCluster1 = new Cluster
        {
            // You can choose SSD or HDD storage type here: StorageType.Ssd or StorageType.Hdd.
            // Cluster storage type can not be changed after the instance is created.
            // If not set will default to SSD type.
            DefaultStorageType = StorageType.Ssd,
            LocationAsLocationName = LocationName.FromProjectLocation(projectId, zone1),
            // Serve Nodes count can only be set if PRODUCTION type instance is being created.
            // Minimum count of 3 serve nodes must be specified.
            // Serve Nodes count can be increased and decreased after an instance is created.
            ServeNodes = 3
        };

        // Initialize request argument(s).
        CreateInstanceRequest request = new CreateInstanceRequest
        {
            ParentAsProjectName = new ProjectName(projectId),
            Instance = myInstance,
            InstanceId = instanceId,
            // Must specify at lease one cluster.
            // Only PRODUCTION type instance can be created with more than one cluster.
            // Currently all clusters must have the same storage type.
            // Clusters must be set to different locations.
            Clusters = { { "ssd-cluster1", myCluster1 } }
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
// [END bigtable_create_prod_instance]
