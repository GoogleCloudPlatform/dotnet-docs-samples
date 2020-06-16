// [START bigtable_create_family_gc_intersection]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;

public class CreateIntersectionFamilySample
{
    public Table CreateIntersectionFamily(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Create a column family with GC policy to drop data that matches all conditions.
        // GC rule: Drop cells older than 5 days AND older than the most recent 2 versions.
        GcRule.Types.Intersection intersectionRule = new GcRule.Types.Intersection
        {
            Rules = {
                new GcRule { MaxNumVersions = 2 },
                new GcRule { MaxAge = Duration.FromTimeSpan(TimeSpan.FromDays(5)) }
            }
        };
        GcRule gcRule = new GcRule { Intersection = intersectionRule };

        // Column family to create
        ColumnFamily columnFamily = new ColumnFamily { GcRule = gcRule };

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        // Modification to create column family
        ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
        {
            Create = columnFamily,
            Id = "cf4"
        };

        Table response = bigtableTableAdminClient.ModifyColumnFamilies(tableName, new List<ModifyColumnFamiliesRequest.Types.Modification> { modification });
        return response;
    }
}
// [END bigtable_create_family_gc_intersection]
