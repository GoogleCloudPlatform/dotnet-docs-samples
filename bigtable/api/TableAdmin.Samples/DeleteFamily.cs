// [START bigtable_delete_family]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

public class DeleteFamilySample
{
    public Table DeleteFamily(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        // Delete a column family.
        ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
        {
            Drop = true,
            Id = "cf2"
        };

        ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
        {
            TableName = tableName,
            Modifications = { modification }
        };

        Table response = bigtableTableAdminClient.ModifyColumnFamilies(tableName, new RepeatedField<ModifyColumnFamiliesRequest.Types.Modification> { modification });
        return response;
    }
}
// [END bigtable_delete_family]
