// Copyright (c) 2019 Google LLC.
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

using System;
using Google.Cloud.Bigtable.V2;
using Google.Cloud.Bigtable.Common.V2;

namespace Writes
{
    public class WriteIncrementSample
    {
        /// <summary>
        /// Increments a cell value in a row with an existing value at that cell.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>
        public string WriteIncrement(
            string projectId = "YOUR-PROJECT-ID",
            string instanceId = "YOUR-INSTANCE-ID",
            string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();

            TableName tableName = new TableName(projectId, instanceId, tableId);
            BigtableByteString rowkey = new BigtableByteString("phone#4c410523#20190501");
            string COLUMN_FAMILY = "stats_summary";

            // Increment the value of stats_summary:connected_wifi by -1 (change 1 to 0 to show it's disconnected)
            ReadModifyWriteRowResponse readModifyWriteRowResponse = bigtableClient.ReadModifyWriteRow(
                tableName,
                rowkey,
                ReadModifyWriteRules.Increment(COLUMN_FAMILY, "connected_wifi", -1));
            return $"Successfully updated row {readModifyWriteRowResponse.Row.Key}";
        }
    }
}
// [END bigtable_writes_increment]
