// [START bigtable_create_family_gc_max_versions]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using System;
using System.Collections.Generic;
using System.Text;

public class CreateMaxVersionsFamilySample
{
    public Table CreateMaxVersionsFamily(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Create a column family with GC policy : most recent N versions
        // where 1 = most recent version
        // Define the GC policy to retain only the most recent 2 versions
        GcRule maxVersionsRule = new GcRule { MaxNumVersions = 2 };

        // Column family to create
        ColumnFamily columnFamily = new ColumnFamily { GcRule = maxVersionsRule };

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        // Modification to create column family
        ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
        {
            Create = columnFamily,
            Id = "cf2"
        };

        ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
        {
            TableName = tableName,
            Modifications = { modification }
        };

        Table response = bigtableTableAdminClient.ModifyColumnFamilies(request);
        return response;
    }
}
// [END bigtable_create_family_gc_max_versions]
