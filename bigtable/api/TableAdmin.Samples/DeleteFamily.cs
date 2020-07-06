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

// [START bigtable_delete_family]

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

public class DeleteFamilySample
{
    public Table DeleteFamily(string projectId, string instanceId, string tableId, string familyId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        // Delete a column family.
        ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
        {
            Drop = true,
            Id = familyId
        };

        Table response = bigtableTableAdminClient.ModifyColumnFamilies(tableName, new RepeatedField<ModifyColumnFamiliesRequest.Types.Modification> { modification });
        return response;
    }
}
// [END bigtable_delete_family]
