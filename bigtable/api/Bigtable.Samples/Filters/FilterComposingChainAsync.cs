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

// [START bigtable_filters_composing_chain]

using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FilterComposingChainAsyncSample
{
    /// <summary>
    /// Read using a chain filter from an existing table.
    ///</summary>
    /// <param name="projectId">Your Google Cloud Project ID.</param>
    /// <param name="instanceId">Your Google Cloud Bigtable Instance ID.</param>
    /// <param name="tableId">Your Google Cloud Bigtable table ID.</param>
    public async Task<List<Row>> FilterComposingChainAsync(string projectId, string instanceId, string tableId)
    {
        BigtableClient bigtableClient = BigtableClient.Create();

        // A filter that selects one cell per column AND within the column family cell_plan
        RowFilter filter = RowFilters.Chain(RowFilters.CellsPerColumnLimit(1), RowFilters.FamilyNameExact("cell_plan"));

        TableName tableName = TableName.FromProjectInstanceTable(projectId, instanceId, tableId);

        ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, filter: filter);
        var enumerator = readRowsStream.GetAsyncEnumerator(default);

        var result = new List<Row>();
        while (await enumerator.MoveNextAsync())
        {
            var row = enumerator.Current;
            result.Add(row); ;
        }
        await enumerator.DisposeAsync();
        return result;
    }
}
// [END bigtable_filters_composing_chain]
