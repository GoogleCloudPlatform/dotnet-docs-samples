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

// [START bigtable_create_table]

using Google.Cloud.Bigtable.Admin.V2;
using Grpc.Core;

public class CreateTableSample
{
    public Table CreateTable(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Table to create
        Table table = new Table
        {
            Granularity = Table.Types.TimestampGranularity.Millis
        };
        var parent = InstanceName.FromProjectInstance(projectId, instanceId);
        try
        {
            table = bigtableTableAdminClient.CreateTable(parent, tableId, table);
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.AlreadyExists)
        {
            // Already exists.  That's fine.
        }

        return table;
    }
}
// [END bigtable_create_table]
