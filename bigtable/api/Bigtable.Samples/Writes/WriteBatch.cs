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

// [START bigtable_writes_batch]

using System;
using Google.Cloud.Bigtable.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;

public class WriteBatchSample
{
    public IEnumerable<MutateRowsResponse.Types.Entry> WriteBatch(string projectId, string instanceId, string tableId)
    {
        BigtableClient bigtableClient = BigtableClient.Create();

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);
        BigtableVersion timestamp = new BigtableVersion(DateTime.UtcNow);
        string COLUMN_FAMILY = "stats_summary";

        MutateRowsRequest.Types.Entry mutations1 = Mutations.CreateEntry(new BigtableByteString("tablet#a0b81f74#20190501"),
            Mutations.SetCell(COLUMN_FAMILY, "connected_cell", 1, timestamp),
            Mutations.SetCell(COLUMN_FAMILY, "os_build", "12155.0.0-rc1", timestamp)
        );
        MutateRowsRequest.Types.Entry mutations2 = Mutations.CreateEntry(new BigtableByteString("tablet#a0b81f74#20190502"),
            Mutations.SetCell(COLUMN_FAMILY, "connected_cell", 1, timestamp),
            Mutations.SetCell(COLUMN_FAMILY, "os_build", "12145.0.0-rc6", timestamp)
        );
        MutateRowsRequest.Types.Entry[] entries = { mutations1, mutations2 };
        MutateRowsResponse mutateRowResponse = bigtableClient.MutateRows(tableName, entries);

        return mutateRowResponse.Entries;
    }
}
// [END bigtable_writes_batch]
