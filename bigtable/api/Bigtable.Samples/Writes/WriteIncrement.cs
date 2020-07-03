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

// [START bigtable_writes_increment]

using Google.Cloud.Bigtable.V2;
using Google.Cloud.Bigtable.Common.V2;

public class WriteIncrementSample
{
    public Row WriteIncrement(string projectId, string instanceId, string tableId)
    {
        BigtableClient bigtableClient = BigtableClient.Create();

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);
        BigtableByteString rowkey = new BigtableByteString("phone#4c410523#20190501");
        string COLUMN_FAMILY = "stats_summary";

        // Increment the value of stats_summary:connected_wifi by -1 (change 1 to 0 to show it's disconnected)
        ReadModifyWriteRowResponse readModifyWriteRowResponse = bigtableClient.ReadModifyWriteRow(tableName, rowkey, ReadModifyWriteRules.Increment(COLUMN_FAMILY, "connected_wifi", -1));
        return readModifyWriteRowResponse.Row;
    }
}
// [END bigtable_writes_increment]
