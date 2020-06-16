
// [START bigtable_create_family_gc_union]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;

public class CreateUnionFamilySample
{
    public Table CreateUnionFamily(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Create a column family with GC policy to drop data that matches at least one condition.
        // Define a GC rule to drop cells older than 5 days or not the most recent version.
        GcRule.Types.Union unionRule = new GcRule.Types.Union
        {
            Rules = {
                new GcRule { MaxNumVersions = 1 },
                new GcRule { MaxAge = Duration.FromTimeSpan(TimeSpan.FromDays(5)) }
            }
        };
        GcRule gcRule = new GcRule { Union = unionRule };

        // Column family to create
        ColumnFamily columnFamily = new ColumnFamily { GcRule = gcRule };

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        // Modification to create column family
        ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
        {
            Create = columnFamily,
            Id = "cf3"
        };

        Table response = bigtableTableAdminClient.ModifyColumnFamilies(tableName, new List<ModifyColumnFamiliesRequest.Types.Modification> { modification });
        return response;
    }
}
// [END bigtable_create_family_gc_union]
