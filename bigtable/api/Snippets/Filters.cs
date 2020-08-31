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


// [START bigtable_filters_print]
// [START bigtable_filters_imports]
using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using System;
using System.Linq;

// [END bigtable_filters_imports]



namespace Filters
{
    public class FilterSnippets
    {

        
        // Write your code here.
        // [START_EXCLUDE]
        // [START bigtable_filters_limit_row_sample]
        /// <summary>
        /// /// Read using a row sample filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitRowSample(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches cells from a row with probability .75
            RowFilter filter = RowFilters.RowSample(.75);
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_row_sample]

        // [START bigtable_filters_limit_row_regex]
        /// <summary>
        /// /// Read using a row regex filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitRowRegex(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches cells from rows whose keys satisfy the given regex
            RowFilter filter = RowFilters.RowKeyRegex(".*#20190501$");
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_row_regex]

        // [START bigtable_filters_limit_cells_per_col]
        /// <summary>
        /// /// Read using a cells per column filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitCellsPerCol(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches only the most recent 2 cells within each column
            RowFilter filter = RowFilters.CellsPerColumnLimit(2);
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_cells_per_col]

        // [START bigtable_filters_limit_cells_per_row]
        /// <summary>
        /// /// Read using a cells per row filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitCellsPerRow(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches the first 2 cells of each row
            RowFilter filter = RowFilters.CellsPerRowLimit(2);
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_cells_per_row]

        // [START bigtable_filters_limit_cells_per_row_offset]
        /// <summary>
        /// /// Read using a cells per row offset filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitCellsPerRowOffset(
    String projectId, String instanceId, String tableId)
        {
            // A filter that skips the first 2 cells per row
            RowFilter filter = RowFilters.CellsPerRowOffset(2);
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_cells_per_row_offset]

        // [START bigtable_filters_limit_col_family_regex]
        /// <summary>
        /// /// Read using a family regex filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitColFamilyRegex(
    String projectId, String instanceId, String tableId)
        {
            // A filter that matches cells whose column family satisfies the given regex
            RowFilter filter = RowFilters.FamilyNameRegex("stats_.*$");
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_col_family_regex]

        // [START bigtable_filters_limit_col_qualifier_regex]
        /// <summary>
        /// /// Read using a qualifier regex filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitColQualifierRegex(
    String projectId, String instanceId, String tableId)
        {
            // A filter that matches cells whose column qualifier satisfies the given regex
            RowFilter filter = RowFilters.ColumnQualifierRegex("connected_.*$");
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_col_qualifier_regex]

        // [START bigtable_filters_limit_col_range]
        /// <summary>
        /// /// Read using a qualifer range filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitColRange(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches cells whose column qualifiers are between data_plan_01gb and
            // data_plan_10gb in the column family cell_plan
            RowFilter filter = RowFilters.ColumnRange(ColumnRange.ClosedOpen("cell_plan", "data_plan_01gb", "data_plan_10gb"));
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_col_range]

        // [START bigtable_filters_limit_value_range]
        /// <summary>
        /// /// Read using a value range filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitValueRange(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches cells whose values are between the given values
            RowFilter filter = RowFilters.ValueRange(ValueRange.Closed("PQ2A.190405", "PQ2A.190406"));
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_value_range]

        // [START bigtable_filters_limit_value_regex]
        /// <summary>
        /// /// Read using a value regex filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitValueRegex(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches cells whose value satisfies the given regex
            RowFilter filter = RowFilters.ValueRegex("PQ2A.*$");
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_value_regex]

        // [START bigtable_filters_limit_timestamp_range]
        /// <summary>
        /// /// Read using a timestamp range filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitTimestampRange(
    String projectId, String instanceId, String tableId)
        {
            BigtableVersion timestamp_minus_hr = new BigtableVersion(new DateTime(2020, 1, 10, 13, 0, 0, DateTimeKind.Utc));

            // A filter that matches cells whose timestamp is from an hour ago or earlier
            RowFilter filter = RowFilters.TimestampRange(new DateTime(0), timestamp_minus_hr.ToDateTime());
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_timestamp_range]

        // [START bigtable_filters_limit_block_all]
        /// <summary>
        /// /// Read using a block all filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitBlockAll(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that does not match any cells
            RowFilter filter = RowFilters.BlockAllFilter();
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_block_all]

        // [START bigtable_filters_limit_pass_all]
        /// <summary>
        /// /// Read using a pass all filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterLimitPassAll(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches all cells
            RowFilter filter = RowFilters.PassAllFilter();
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_limit_pass_all]

        // [START bigtable_filters_modify_strip_value]
        /// <summary>
        /// /// Read using a strip value filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterModifyStripValue(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that replaces the outputted cell value with the empty string
            RowFilter filter = RowFilters.StripValueTransformer();
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_modify_strip_value]

        // [START bigtable_filters_modify_apply_label]
        /// <summary>
        /// /// Read using a strip value filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterModifyApplyLabel(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that applies the given label to the outputted cell
            RowFilter filter = new RowFilter { ApplyLabelTransformer = "labelled" };
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_modify_apply_label]

        // [START bigtable_filters_composing_chain]
        /// <summary>
        /// /// Read using a chain filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterComposingChain(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that selects one cell per column AND within the column family cell_plan
            RowFilter filter = RowFilters.Chain(RowFilters.CellsPerColumnLimit(1), RowFilters.FamilyNameExact("cell_plan"));
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_composing_chain]

        // [START bigtable_filters_composing_interleave]
        /// <summary>
        /// /// Read using an interleave filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterComposingInterleave(
    String projectId, String instanceId, String tableId)
        {
            // A filter that matches cells with the value true OR with the column qualifier os_build
            RowFilter filter = RowFilters.Interleave(RowFilters.ValueExact("true"), RowFilters.ColumnQualifierExact("os_build"));
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_composing_interleave]

        // [START bigtable_filters_composing_condition]
        /// <summary>
        /// /// Read using a conditional filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>

        public string filterComposingCondition(string projectId = "YOUR-PROJECT-ID", string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that applies the label passed-filter IF the cell has the column qualifier
            // data_plan_10gb AND the value true, OTHERWISE applies the label filtered-out
            RowFilter filter = RowFilters.Condition(
                RowFilters.Chain(RowFilters.ColumnQualifierExact("data_plan_10gb"), RowFilters.ValueExact("true")),
                new RowFilter { ApplyLabelTransformer = "passed-filter" },
                new RowFilter { ApplyLabelTransformer = "filtered-out" }
                );
            return readFilter(projectId, instanceId, tableId, filter);
        }
        // [END bigtable_filters_composing_condition]


        // [END_EXCLUDE]
        public string readFilter(string projectId, string instanceId, string tableId, RowFilter filter)
        {
            BigtableClient bigtableClient = BigtableClient.Create();
            TableName tableName = new TableName(projectId, instanceId, tableId);

            ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, filter: filter);
            string result = "";
            readRowsStream.ForEach(row => result += printRow(row));
            if (result == "")
            {
                return "empty";
            }
            return result;
        }

        public string printRow(Row row)
        {
            String result = $"Reading data for {row.Key.ToStringUtf8()}\n";
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
// [END bigtable_filters_print]