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

using Google.Cloud.Bigtable.Admin.V2;
using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

[CollectionDefinition(nameof(BigtableClientFixture))]
public class BigtableClientFixture : IDisposable, ICollectionFixture<BigtableClientFixture>
{
    public string ProjectId { get; private set; } = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    public string InstanceId { get; private set; } = "lalji-test-instance";    // Environment.GetEnvironmentVariable("TEST_BIGTABLE_INSTANCE");
    public string TableId { get; private set; } = $"mobile-time-series-{Guid.NewGuid().ToString().Substring(0, 8)}";

    public BigtableClientFixture()
    {
        InitializeTable();
        InitializeData();
    }

    private void InitializeTable()
    {
        var bigtableTableAdminClient = BigtableTableAdminClient.Create();
        Table table = new Table
        {
            Granularity = Table.Types.TimestampGranularity.Millis
        };
        table.ColumnFamilies.Add("stats_summary", new ColumnFamily());
        table.ColumnFamilies.Add("cell_plan", new ColumnFamily());
        CreateTableRequest createTableRequest = new CreateTableRequest
        {
            ParentAsInstanceName = new InstanceName(ProjectId, InstanceId),
            Table = table,
            TableId = TableId,
        };
        bigtableTableAdminClient.CreateTable(createTableRequest);
    }

    private void InitializeData()
    {
        BigtableClient bigtableClient = BigtableClient.Create();
        TableName tableName = new TableName(ProjectId, InstanceId, TableId);
        BigtableVersion timestamp = new BigtableVersion(new DateTime(2020, 1, 10, 14, 0, 0, DateTimeKind.Utc));
        BigtableVersion timestamp_minus_hr = new BigtableVersion(new DateTime(2020, 1, 10, 13, 0, 0, DateTimeKind.Utc));

        MutateRowsRequest.Types.Entry[] entries = {
            Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190501"),
                Mutations.SetCell("cell_plan", "data_plan_01gb", "false", timestamp),
                Mutations.SetCell("cell_plan", "data_plan_01gb", "true", timestamp_minus_hr),
                Mutations.SetCell("cell_plan", "data_plan_05gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", 1, timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190405.004", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190502"),
                Mutations.SetCell("cell_plan", "data_plan_05gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190405.004", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190505"),
                Mutations.SetCell("cell_plan", "data_plan_05gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "0", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190406.000", timestamp)),
                 Mutations.CreateEntry(new BigtableByteString("phone#5c10102#20190501"),
                Mutations.SetCell("cell_plan", "data_plan_10gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190401.002", timestamp)),
            Mutations.CreateEntry(new BigtableByteString("phone#5c10102#20190502"),
                Mutations.SetCell("cell_plan", "data_plan_10gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "0", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190406.000", timestamp))
                };

        bigtableClient.MutateRows(tableName, entries);
    }

    public void Dispose()
    {
        var bigtableTableAdminClient = BigtableTableAdminClient.Create();
        bigtableTableAdminClient.DeleteTable(TableName.FromProjectInstanceTable(ProjectId, InstanceId, TableId));
    }

    public string GetRowData(Row row)
    {
        StringBuilder result = new StringBuilder($"Reading data for {row.Key.ToStringUtf8()}\n");
        foreach (Family family in row.Families)
        {
            result.Append($"Column Family {family.Name}\n");

            foreach (Column column in family.Columns)
            {
                foreach (Cell cell in column.Cells)
                {
                    result.Append($"\t{column.Qualifier.ToStringUtf8()}: {cell.Value.ToStringUtf8()} @{cell.TimestampMicros} {cell.Labels}\n");
                }
            }
        }
        result.Append("\n");
        return result.ToString();
    }

    public string GetRowsData(List<Row> rows)
    {
        StringBuilder result = new StringBuilder();
        rows.ForEach(row => result.Append(GetRowData(row)));
        return result.ToString();
    }
}
