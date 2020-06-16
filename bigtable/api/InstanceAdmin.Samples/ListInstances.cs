// [START bigtable_list_instances]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Bigtable.Admin.V2;
using System;
using System.Collections.Generic;
using System.Text;

public class ListInstancesSample
{
    public IEnumerable<Instance> ListInstances(string projectId)
    {
        BigtableInstanceAdminClient bigtableInstanceAdminClient = BigtableInstanceAdminClient.Create();

        // Lists instances in the project.
        ProjectName projectName = ProjectName.FromProject(projectId);
        ListInstancesResponse response = bigtableInstanceAdminClient.ListInstances(projectName);
        return response.Instances;
    }
}
// [END bigtable_list_instances]
