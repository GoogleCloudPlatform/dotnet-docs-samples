// [START bigtable_create_cluster]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Bigtable.Admin.V2;
using Google.LongRunning;
using System;
using System.Collections.Generic;
using System.Text;

public class CreateClusterSample
{
    public static object CreateCluster(string projectId, string instanceId, string clusterId)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        // Please refer to the link below for the full list of availabel locations:
        // https://cloud.google.com/bigtable/docs/locations
        string zone2 = "us-east1-d";

        // Create an additional cluster with cluster id "ssd-cluster2" with 3 nodes and location us-east1-d.
        // Additional cluster can only be created in PRODUCTION type instance.
        // Additional cluster must have same storage type as existing cluster.
        // Please read about routing_policy for more information on mutli cluster instances.
        // https://cloud.google.com/bigtable/docs/reference/admin/rpc/google.bigtable.admin.v2#google.bigtable.admin.v2.AppProfile.MultiClusterRoutingUseAny
        // Cluster to be created within the instance.
        Cluster myCluster2 = new Cluster
        {
            DefaultStorageType = StorageType.Ssd,
            LocationAsLocationName = LocationName.FromProjectLocation(projectId, zone2),
            ServeNodes = 3
        };

        // Initialize request argument(s).
        CreateClusterRequest request = new CreateClusterRequest
        {
            ParentAsInstanceName = InstanceName.FromProjectInstance(projectId, instanceId),
            ClusterId = clusterId,
            Cluster = myCluster2
        };
        Operation<Cluster, CreateClusterMetadata> response = bigtableInstanceAdminClient.CreateCluster(request);

        Console.WriteLine("Waiting for operation to complete...");
        // Poll until the returned long-running operation is complete
        Operation<Cluster, CreateClusterMetadata> completedResponse = response.PollUntilCompleted();

        return completedResponse.Result;
    }
}
// [END bigtable_create_cluster]
