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

// [START bigtable_reads_row]

using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;

namespace Reads
{
    public class ReadRow
    {
        /// <summary>
        /// Reads one row from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>
        public static string BigtableReadRow(
            string projectId = "YOUR-PROJECT-ID",
            string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);
            Row row = bigtableClient.ReadRow(tableName, rowKey: "phone#4c410523#20190501");

            return PrintRow.Print(row);
        }
    }
}
// [END bigtable_reads_row]
