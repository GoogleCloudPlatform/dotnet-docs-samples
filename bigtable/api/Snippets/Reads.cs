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


// [START bigtable_reads_print]
// [START bigtable_reads_imports]
using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using System;
using System.Linq;
using System.Threading.Tasks;
// [END bigtable_reads_imports]



namespace Reads
{
    public class ReadSnippets
    {
        // Write your code here.
        // [START_EXCLUDE]
        // [START bigtable_reads_row]

        /// <summary>
        /// /// Reads one row from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>
        public string ReadRow(
            string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);
            Row row = bigtableClient.ReadRow(tableName, rowKey: "phone#4c410523#20190501");

            return PrintRow(row);
        }
        // [END bigtable_reads_row]


        // [START bigtable_reads_row_partial]

        /// <summary>
        /// /// Reads part of one row from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string ReadRowPartial(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);
            string rowkey = "phone#4c410523#20190501";
            RowFilter filter = RowFilters.ColumnQualifierExact("os_build");
            Row row = bigtableClient.ReadRow(tableName, rowkey, filter);
            return PrintRow(row);
        }
        // [END bigtable_reads_row_partial]

        // [START bigtable_reads_rows]


        /// <summary>
        /// /// Reads multiple rows from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>
        public async Task<string> ReadRows(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);

            RowSet rowSet = RowSet.FromRowKeys("phone#4c410523#20190501", "phone#4c410523#20190502");
            ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, rowSet);

            string result = "";
            await foreach(var row in readRowsStream)
            {
                result += PrintRow(row);
            }

            return result;
        }
        // [END bigtable_reads_rows]

        // [START bigtable_reads_row_range]

        /// <summary>
        /// /// Reads a range of rows from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public async Task<string> ReadRowRange(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            string start = "phone#4c410523#20190501";
            string end = "phone#4c410523#201906201";

            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);
            RowSet rowSet = RowSet.FromRowRanges(RowRange.ClosedOpen(start, end));
            ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, rowSet);

            string result = "";
            await foreach (var row in readRowsStream)
            {
                result += PrintRow(row);
            }

            return result;
        }
        // [END bigtable_reads_row_range]

        // [START bigtable_reads_row_ranges]

        /// <summary>
        /// /// Reads multiple ranges of rows from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public async Task<string> ReadRowRanges(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);
            RowSet rowSet = RowSet.FromRowRanges(RowRange.ClosedOpen("phone#4c410523#20190501", "phone#4c410523#20190601"),
            RowRange.ClosedOpen("phone#5c10102#20190501", "phone#5c10102#20190601"));
            ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, rowSet);

            string result = "";
            await foreach (var row in readRowsStream)
            {
                result += PrintRow(row);
            }

            return result;
        }
        // [END bigtable_reads_row_ranges]

        // [START bigtable_reads_prefix]

        /// <summary>
        /// /// Reads rows starting with a prefix from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>
        public async Task<string> ReadPrefix(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);

            string prefix = "phone";
            char prefixEndChar = prefix[prefix.Length - 1];
            prefixEndChar++;
            string end = prefix.Substring(0, prefix.Length - 1) + prefixEndChar;
            RowSet rowSet = RowSet.FromRowRanges(RowRange.Closed(prefix, end));
            ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, rowSet);

            string result = "";
            await foreach (var row in readRowsStream)
            {
                result += PrintRow(row);
            }

            return result;
        }
        // [END bigtable_reads_prefix]

        // [START bigtable_reads_filter]

        /// <summary>
        /// /// Reads using a filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public async Task<string> ReadFilter(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);

            RowFilter filter = RowFilters.ValueRegex("PQ2A.*");

            ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, filter: filter);
            string result = "";
            await foreach (var row in readRowsStream)
            {
                result += PrintRow(row);
            }

            return result;
        }
        // [END bigtable_reads_filter]

        // [END_EXCLUDE]
        public string PrintRow(Row row)
        {
            string result = $"Reading data for {row.Key.ToStringUtf8()}\n";
            foreach (Family family in row.Families)
            {
                result += $"Column Family {family.Name}\n";

                foreach (Column column in family.Columns)
                {
                    foreach (Cell cell in column.Cells)
                    {
                        result += $"\t{column.Qualifier.ToStringUtf8()}: {cell.Value.ToStringUtf8()} @{cell.TimestampMicros} {cell.Labels}\n";
                    }
                }
            }
            result += "\n";
            Console.WriteLine(result);
            return result;
        }
    }
}
// [END bigtable_reads_print]
