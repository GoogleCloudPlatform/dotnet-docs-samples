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
    public readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    public readonly string instanceId = "lalji-test-instance";    // Environment.GetEnvironmentVariable("TEST_BIGTABLE_INSTANCE");
    public readonly string tableId = $"mobile-time-series-{Guid.NewGuid().ToString().Substring(0, 8)}";

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
            ParentAsInstanceName = new InstanceName(projectId, instanceId),
            Table = table,
            TableId = tableId,
        };
        bigtableTableAdminClient.CreateTable(createTableRequest);
    }

    private void InitializeData()
    {
        BigtableClient bigtableClient = BigtableClient.Create();
        TableName tableName = new TableName(projectId, instanceId, tableId);
        BigtableVersion timestamp = new BigtableVersion(new DateTime(2020, 1, 10, 14, 0, 0, DateTimeKind.Utc));
        BigtableVersion timestamp_minus_hr = new BigtableVersion(new DateTime(2020, 1, 10, 13, 0, 0, DateTimeKind.Utc));

        MutateRowsRequest.Types.Entry[] entries = {
            Mutations.CreateEntry(new BigtableByteString("phone#4c410523#20190501"),
                Mutations.SetCell("cell_plan", "data_plan_01gb", "false", timestamp),
                Mutations.SetCell("cell_plan", "data_plan_01gb", "true", timestamp_minus_hr),
                Mutations.SetCell("cell_plan", "data_plan_05gb", "true", timestamp),
                Mutations.SetCell("stats_summary", "connected_cell", "1", timestamp),
                Mutations.SetCell("stats_summary", "connected_wifi", "1", timestamp),
                Mutations.SetCell("stats_summary", "os_build", "PQ2A.190405.003", timestamp)),
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
        bigtableTableAdminClient.DeleteTable(TableName.FromProjectInstanceTable(projectId, instanceId, tableId));
    }
}
