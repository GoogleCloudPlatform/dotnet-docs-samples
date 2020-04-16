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

using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using System.Threading.Tasks;

namespace Filters
{
    public class ReadFilter
    {
        /// <summary>
        /// Bigtables the read filter.
        /// </summary>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="instanceId">The instance identifier.</param>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="filter">The filter.</param>
        public static async Task<string> BigtableReadFilter(string projectId,
            string instanceId, string tableId, RowFilter filter)
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);

            ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, filter: filter);
            var enumerator = readRowsStream.GetAsyncEnumerator(default);

            string result = "";

            while (await enumerator.MoveNextAsync())
            {
                var row = enumerator.Current;
                result += PrintRow.Print(row);
            }

            if (result == "")
            {
                return "empty";
            }
            return result;
        }
    }
}
