// [START bigtable_create_family_gc_max_age]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

public class CreateMaxAgeFamilySample
{
    public Table CreateMaxAgeFamily(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Create a column family with GC policy : maximum age
        // where age = current time minus cell timestamp
        // Define the GC rule to retain data with max age of 5 days
        GcRule MaxAgeRule = new GcRule { MaxAge = Duration.FromTimeSpan(TimeSpan.FromDays(5.0)) };

        // Column family to create
        ColumnFamily columnFamily = new ColumnFamily { GcRule = MaxAgeRule };

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        // Modification to create column family
        ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
        {
            Create = columnFamily,
            Id = "cf1"
        };
        Table response = bigtableTableAdminClient.ModifyColumnFamilies(tableName, new List<ModifyColumnFamiliesRequest.Types.Modification> { modification });
        return response;
    }
}
// [END bigtable_create_family_gc_max_age]
