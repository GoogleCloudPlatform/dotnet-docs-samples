
// [START bigtable_delete_instance]

using Google.Cloud.Bigtable.Admin.V2;
using System;
using System.Collections.Generic;
using System.Text;

public class DeleteInstanceSample
{
    public void DeleteInstance(string projectId, string instanceId)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        InstanceName instanceName = InstanceName.FromProjectInstance(projectId, instanceId);
        bigtableInstanceAdminClient.DeleteInstance(instanceName);
    }
}
// [END bigtable_delete_instance]
