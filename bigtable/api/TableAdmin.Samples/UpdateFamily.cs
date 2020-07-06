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

// [START bigtable_update_gc_rule]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.Collections;

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
