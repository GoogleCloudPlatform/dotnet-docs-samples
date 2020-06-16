// [START bigtable_list_clusters]

using Google.Cloud.Bigtable.Admin.V2;
using System;
using System.Collections.Generic;
using System.Text;

public class ListClustersSample
{
    public static IEnumerable<Cluster> ListClusters(string projectId, string instanceId)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        InstanceName instanceName = InstanceName.FromProjectInstance(projectId, instanceId);
        // Lists clusters in the instance.
        ListClustersResponse response = bigtableInstanceAdminClient.ListClusters(instanceName);
        return response.Clusters;
    }
}
// [END bigtable_list_clusters]
