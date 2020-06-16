// [START bigtable_create_family_gc_nested]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;

public class CreateNestedFamilySample
{
    public Table CreateNestedFamily(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Create a nested GC rule:
        // Drop cells that are either older than the 10 recent versions
        // OR
        // Drop cells that are older than 5 days AND older than the 2 recent versions.
        GcRule.Types.Intersection intersectionRule = new GcRule.Types.Intersection
        {
            Rules = {
                new GcRule { MaxNumVersions = 2 },
                new GcRule { MaxAge = Duration.FromTimeSpan(TimeSpan.FromDays(5)) }
            }
        };

        GcRule.Types.Union nestedRule = new GcRule.Types.Union
        {
            Rules = {
                new GcRule { MaxNumVersions = 10 },
                new GcRule { Intersection = intersectionRule }
            }
        };

        GcRule gcRule = new GcRule { Union = nestedRule };

        // Column family to create
        ColumnFamily columnFamily = new ColumnFamily { GcRule = gcRule };

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        // Modification to create column family
        ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
        {
            Create = columnFamily,
            Id = "cf5"
        };

        Table response = bigtableTableAdminClient.ModifyColumnFamilies(tableName, new List<ModifyColumnFamiliesRequest.Types.Modification> { modification });

        return response;
    }
}
// [END bigtable_create_family_gc_nested]
