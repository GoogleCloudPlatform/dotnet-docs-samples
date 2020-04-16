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

// [START bigtable_reads_prefix]

using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using System.Threading.Tasks;

namespace Reads
{
    public class ReadPrefix
    {
        /// <summary>
        /// Reads rows starting with a prefix from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>
        public static async Task<string> BigtableReadPrefix(
            string projectId = "YOUR-PROJECT-ID",
            string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);

            string prefix = "phone";
            var prefixEndChar = prefix[prefix.Length - 1];
            prefixEndChar++;
            string end = prefix.Substring(0, prefix.Length - 1) + prefixEndChar;
            RowSet rowSet = RowSet.FromRowRanges(RowRange.Closed(prefix, end));
            ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, rowSet);
            var enumerator = readRowsStream.GetAsyncEnumerator(default);

            string result = "";
            while (await enumerator.MoveNextAsync())
            {
                var row = enumerator.Current;
                result += PrintRow.Print(row);
            }

            return result;
        }
    }
}
// [END bigtable_reads_prefix]
