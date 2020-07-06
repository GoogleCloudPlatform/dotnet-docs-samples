// Copyright (c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START bigtable_create_family_gc_union]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

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
