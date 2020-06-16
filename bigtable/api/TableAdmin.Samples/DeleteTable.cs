// [START bigtable_delete_table]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using System;
using System.Collections.Generic;
using System.Text;

public class DeleteTableSample
{
    public void DeleteTable(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Table to delete
        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);
        bigtableTableAdminClient.DeleteTable(tableName);
    }
}
// [END bigtable_delete_table]

