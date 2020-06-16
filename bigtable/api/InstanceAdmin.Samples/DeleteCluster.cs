// [START bigtable_delete_cluster]

using Google.Cloud.Bigtable.Admin.V2;
using System;
using System.Collections.Generic;
using System.Text;

public class DeleteClusterSample
{
    public void DeleteCluster(string projectId, string instanceId, string clusterId)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        ClusterName clusterName = ClusterName.FromProjectInstanceCluster(projectId, instanceId, clusterId);
        bigtableInstanceAdminClient.DeleteCluster(clusterName);
    }
}
// [END bigtable_delete_cluster]
