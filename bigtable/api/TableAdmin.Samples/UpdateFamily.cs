// [START bigtable_update_gc_rule]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

public class UpdateFamilySample
{
    public Table UpdateFamily(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Update the column family metadata to update the GC rule.
        // Updated column family GC rule.
        GcRule maxVersionsRule = new GcRule { MaxNumVersions = 1 };

        // Column family to create
        ColumnFamily columnFamily = new ColumnFamily { GcRule = maxVersionsRule };

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        // Modification to update column family
        ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
        {
            Update = columnFamily,
            Id = "cf1"
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
// [END bigtable_update_gc_rule]
