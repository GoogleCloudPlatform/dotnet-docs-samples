// [START bigtable_get_table]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;

public class GetTableSample
{
    public Table GetTable(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
        var tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);
        
        Table table = bigtableTableAdminClient.GetTable(tableName);
        return table;
    }
}
// [END bigtable_get_table]
