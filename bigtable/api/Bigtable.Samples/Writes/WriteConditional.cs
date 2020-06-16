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

// [START bigtable_writes_conditional]

using System;
using Google.Cloud.Bigtable.V2;
using Google.Cloud.Bigtable.Common.V2;

public class WriteConditionalSample
{
    public bool WriteConditional(string projectId, string instanceId, string tableId)
    {
        BigtableClient bigtableClient = BigtableClient.Create();

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);
        BigtableByteString rowkey = new BigtableByteString("phone#4c410523#20190501");
        BigtableVersion timestamp = new BigtableVersion(DateTime.UtcNow);
        string COLUMN_FAMILY = "stats_summary";

        CheckAndMutateRowResponse checkAndMutateRowResponse = bigtableClient.CheckAndMutateRow(tableName, rowkey,
            RowFilters.Chain(RowFilters.FamilyNameExact(COLUMN_FAMILY),
            RowFilters.ColumnQualifierExact("os_build"),
            RowFilters.ValueRegex("PQ2A\\..*")),
            Mutations.SetCell(COLUMN_FAMILY, "os_name", "android", timestamp));

        return checkAndMutateRowResponse.PredicateMatched;
    }
}
// [END bigtable_writes_conditional]
