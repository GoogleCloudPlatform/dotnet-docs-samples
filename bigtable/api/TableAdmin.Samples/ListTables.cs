// [START bigtable_list_tables]

using Google.Cloud.Bigtable.Admin.V2;
using System.Collections.Generic;

public class ListTablesSample
{
    public IEnumerable<Table> ListTables(string projectId, string instanceId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
        var parent = InstanceName.FromProjectInstance(projectId, instanceId);
        var tables = bigtableTableAdminClient.ListTables(parent);
        return tables;
    }
}
// [END bigtable_list_tables]
