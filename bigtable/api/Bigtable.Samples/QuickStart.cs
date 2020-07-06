// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START bigtable_quickstart]

using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using System;

public class QuickStart
{
    public Row DoQuickStart(string projectId, string instanceId, string tableId)
    {
        try
        {
            // Creates a Bigtable client
            BigtableClient bigtableClient = BigtableClient.Create();

            // Read a row from table using a row key
            Row row = bigtableClient.ReadRow(new TableName(projectId, instanceId, tableId), "phone#4c410523#20190501", RowFilters.CellsPerRowLimit(1));
            // Print the row key and data (column value, labels, timestamp)
            Console.WriteLine($"{"Row key:",-30}{row.Key.ToStringUtf8()}\n" +
                              $"{"  Column Family:",-30}{row.Families[0].Name}\n" +
                              $"{"    Column Qualifyer:",-30}{row.Families[0].Columns[0].Qualifier.ToStringUtf8()}\n" +
                              $"{"      Value:",-30}{row.Families[0].Columns[0].Cells[0].Value.ToStringUtf8()}\n" +
                              $"{"      Labels:",-30}{row.Families[0].Columns[0].Cells[0].Labels}\n" +
                              $"{"      Timestamp:",-30}{row.Families[0].Columns[0].Cells[0].TimestampMicros}\n");
            return row;
        }
        catch (Exception ex)
        {
            // Handle error performing the read operation
            Console.WriteLine($"Error reading row r1: {ex.Message}");
            throw;
        }
    }
}
// [END bigtable_quickstart]
