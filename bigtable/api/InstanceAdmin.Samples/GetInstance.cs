// [START bigtable_get_instance]

using Google.Cloud.Bigtable.Admin.V2;
using System;
using System.Collections.Generic;
using System.Text;

public class GetInstanceSample
{
    public Instance GetInstance(string projectId, string instanceId)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        InstanceName instanceName = InstanceName.FromProjectInstance(projectId, instanceId);
        Instance instance = bigtableInstanceAdminClient.GetInstance(instanceName);

        return instance;
    }
    // [END bigtable_get_instance]
}
