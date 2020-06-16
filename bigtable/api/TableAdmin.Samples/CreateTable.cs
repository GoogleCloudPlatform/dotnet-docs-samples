// [START bigtable_create_table]

using Google.Cloud.Bigtable.Admin.V2;
using Grpc.Core;

public class CreateTableSample
{
    public Table CreateTable(string projectId, string instanceId, string tableId)
    {
        BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

        // Table to create
        Table table = new Table
        {
            Granularity = Table.Types.TimestampGranularity.Millis
        };
        var parent = InstanceName.FromProjectInstance(projectId, instanceId);
        try
        {
            table = bigtableTableAdminClient.CreateTable(parent, tableId, table);
        }
        catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.AlreadyExists)
        {
            // Already exists.  That's fine.
        }

        return table;
    }
}
// [END bigtable_create_table]
