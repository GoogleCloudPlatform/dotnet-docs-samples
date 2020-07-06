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
