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

// [START bigtable_reads_row_partial]

using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;

public class ReadRowPartialSample
{
    public Row ReadRowPartial(string projectId, string instanceId, string tableId)
    {
        BigtableClient bigtableClient = BigtableClient.Create();
        TableName tableName = new TableName(projectId, instanceId, tableId);
        string rowkey = "phone#4c410523#20190501";
        RowFilter filter = RowFilters.ColumnQualifierExact("os_build");
        Row row = bigtableClient.ReadRow(tableName, rowkey, filter);
        return row;
    }
}
// [END bigtable_reads_row_partial]
