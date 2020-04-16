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

// [START bigtable_filters_limit_col_range]

using Google.Cloud.Bigtable.V2;
using System.Threading.Tasks;

namespace Filters
{
    public class FilterLimitColRange
    {
        /// <summary>
        /// Read using a qualifer range filter from an existing table.
        ///</summary>
        /// <param name="projectId">Your Google Cloud Project ID.</param>
        /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
        /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>
        public static Task<string> BigtableFilterLimitColRange(
            string projectId = "YOUR-PROJECT-ID",
            string instanceId = "YOUR-INSTANCE-ID", string tableId = "YOUR-TABLE-ID")
        {
            // A filter that matches cells whose column qualifiers are between data_plan_01gb and
            // data_plan_10gb in the column family cell_plan
            RowFilter filter = RowFilters.ColumnRange(ColumnRange.ClosedOpen("cell_plan", "data_plan_01gb", "data_plan_10gb"));
            return ReadFilter.BigtableReadFilter(projectId, instanceId, tableId, filter);
        }
    }
}
// [END bigtable_filters_limit_col_range]
