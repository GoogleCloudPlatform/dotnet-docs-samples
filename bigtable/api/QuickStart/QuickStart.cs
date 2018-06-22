// [START bigtable_quickstart]

using System;
// Imports the Google Cloud client library
using Google.Cloud.Bigtable.V2;

namespace GoogleCloudSamples.Bigtable
{
    public class QuickStart
    {
        public static int Main(string[] args)
        {
            // Your Google Cloud Platform project ID
            const string projectId = "YOUR-PROJECT-ID";
            // The name of the Cloud Bigtable instance
            const string instanceId = "YOUR-INSTANCE-ID";
            // [END bigtable_quickstart]
            if (projectId == "YOUR-PROJECT" + "-ID")
            {
                Console.WriteLine("Edit QuickStart.cs and replace YOUR-PROJECT-ID with your project id.");
                return -1;
            }
            if (instanceId == "YOUR-INSTANCE" + "-ID")
            {
                Console.WriteLine("Edit QuickStart.cs and replace YOUR-INSTANCE-ID with your instance id.");
                return -1;
            }
            // [START bigtable_quickstart]
            // The name of the Cloud Bigtable table
            const string tableId = "my-table";

            try
            {
                // Creates a Bigtable client
                BigtableClient bigtableClient = BigtableClient.Create();

                // Read a row from my-table using a row key
                Row row = bigtableClient.ReadRow(
                    new TableName(projectId, instanceId, tableId), "r1", RowFilters.CellsPerRowLimit(1));
                // Print the row key and data (column value, labels, timestamp)
                Console.WriteLine($"{"Row key:",-30}{row.Key.ToStringUtf8()}\n" +
                                  $"{"  Column Family:",-30}{row.Families[0].Name}\n" +
                                  $"{"    Column Qualifyer:",-30}{row.Families[0].Columns[0].Qualifier.ToStringUtf8()}\n" +
                                  $"{"      Value:",-30}{row.Families[0].Columns[0].Cells[0].Value.ToStringUtf8()}\n" +
                                  $"{"      Labels:",-30}{row.Families[0].Columns[0].Cells[0].Labels}\n" +
                                  $"{"      Timestamp:",-30}{row.Families[0].Columns[0].Cells[0].TimestampMicros}\n");
            }
            catch (Exception ex)
            {
                // Handle error performing the read operation
                Console.WriteLine($"Error reading row r1: {ex.Message}");
            }
            return 0;
        }
    }
}
// [END bigtable_quickstart]

