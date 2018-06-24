// Copyright 2018 Google Inc.
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

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Google.Cloud.Bigtable.V2;
using Google.Cloud.Bigtable.Admin.V2;

namespace GoogleCloudSamples.Bigtable
{
    public class HelloWorld
    {
        // Set up some Cloud Bigtable metadata for convinience
        // Your Google Cloud Platform project ID
        private const string projectId = "YOUR-PROJECT-ID";

        // You Google Cloud Bigtable instance ID
        private const string instanceId = "YOUR-INSTANCE-ID";

        // The name of a table.
        private const string tableId = "Hello-Bigtable";
        // The name of a column family.
        private const string columnFamily = "cf";
        // The name of a culomn inside the column family.
        private const string columnName = "greeting";
        // Some friendly greetings to write to Cloud Bigtable
        private static readonly string[] s_greetings = { "Hello World!", "Hellow Bigtable!", "Hellow C#!" };
        // This List containce mapping indices from MutateRowsRequest to s_greetings[].
        private static List<int> s_mapToOriginalGreetingIndex;
        private const string rowKeyPrefix = "greeting";
        private static int s_greetingIndex;

        private static void DoHelloWorld()
        {
            try
            {
                // BigtableTableAdminClient API lets us create, manage and delete tables.
                BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

                // Create a table with a single column family.
                Console.WriteLine($"Create new table: {tableId} with column family: {columnFamily}, Instance: {instanceId}");

                // Check whether a table with given TableName already exists.
                if (!TableExist(bigtableTableAdminClient))
                {
                    bigtableTableAdminClient.CreateTable(
                        new InstanceName(projectId, instanceId),
                        tableId,
                        new Table
                        {
                            Granularity = Table.Types.TimestampGranularity.Millis,
                            ColumnFamilies =
                            {
                                {
                                    columnFamily, new ColumnFamily
                                    {
                                        GcRule = new GcRule
                                        {
                                            MaxNumVersions = 1
                                        }
                                    }
                                }
                            }
                        });
                    // Confirm that table was created successfully.
                    Console.WriteLine(TableExist(bigtableTableAdminClient)
                        ? $"Table {tableId} created succsessfully\n"
                        : $"There was a problem creating a table {tableId}");
                }
                else
                {
                    Console.WriteLine($"Table: {tableId} already exist");
                }

                // BigtableClient API lets us read and write to a table.
                BigtableClient bigtableClient = BigtableClient.Create();

                // Initialise Google.Cloud.Bigtable.V2.TableName object.
                Google.Cloud.Bigtable.V2.TableName tableNameClient = new Google.Cloud.Bigtable.V2.TableName(projectId, instanceId, tableId);

                // Write some rows
                /* Each row has a unique row key.
                                       
                       Note: This example uses sequential numeric IDs for simplicity, but
                       this can result in poor performance in a production application.
                       Since rows are stored in sorted order by key, sequential keys can
                       result in poor distribution of operations across nodes.
                     
                       For more information about how to design a Bigtable schema for the
                       best performance, see the documentation:
                      
                       https://cloud.google.com/bigtable/docs/schema-design */

                Console.WriteLine($"Write some greetings to the table {tableId}");

                // Insert 1 row using MutateRow()
                s_greetingIndex = 0;
                try
                {
                    bigtableClient.MutateRow(tableNameClient, rowKeyPrefix + s_greetingIndex, MutationBuilder(s_greetingIndex));
                    Console.WriteLine($"\tGreeting:   -- {s_greetings[s_greetingIndex],-18}-- written successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\tFailed to write Greeting: --{s_greetings[s_greetingIndex]}");
                    Console.WriteLine(ex.Message);
                    throw;
                }

                // Insert multiple rows using MutateRows()
                // Build a MutateRowsRequest (contains table name and a collection of entries).
                MutateRowsRequest request = new MutateRowsRequest
                {
                    TableNameAsTableName = tableNameClient
                };

                s_mapToOriginalGreetingIndex = new List<int>();
                while (++s_greetingIndex < s_greetings.Length)
                {
                    s_mapToOriginalGreetingIndex.Add(s_greetingIndex);
                    // Build an entry for every greeting (consists of rowkey and a collection of mutations).
                    string rowKey = rowKeyPrefix + s_greetingIndex;
                    request.Entries.Add(Mutations.CreateEntry(rowKey, MutationBuilder(s_greetingIndex)));
                }

                // Make the request to write multiple rows.
                MutateRowsResponse response = bigtableClient.MutateRows(request);

                // Check the Status code of each entry to insure that it was written successfully
                foreach (MutateRowsResponse.Types.Entry entry in response.Entries)
                {
                    s_greetingIndex = s_mapToOriginalGreetingIndex[(int)entry.Index];
                    if (entry.Status.Code == 0)
                    {
                        Console.WriteLine($"\tGreeting:   -- {s_greetings[s_greetingIndex],-18}-- written successfully");
                    }
                    else
                    {
                        Console.WriteLine($"\tFailed to write Greeting: --{s_greetings[s_greetingIndex]}");
                        Console.WriteLine(entry.Status.Message);
                    }
                }

                // Read from the table.
                Console.WriteLine("Read the first row");

                int rowIndex = 0;

                // Read a specific row. Apply filter to return latest only cell value accross entire row.
                Row rowRead = bigtableClient.ReadRow(
                    tableNameClient, rowKey: rowKeyPrefix + rowIndex, filter: RowFilters.CellsPerRowLimit(1));
                Console.WriteLine(
                    $"\tRow key: {rowRead.Key.ToStringUtf8()} " +
                    $"  -- Value: {rowRead.Families[0].Columns[0].Cells[0].Value.ToStringUtf8(),-16} " +
                    $"  -- Time Stamp: {rowRead.Families[0].Columns[0].Cells[0].TimestampMicros}");

                Console.WriteLine("Read all rows using streaming");
                // stream the content of the whole table. Apply filter to return latest only cell values accross all rows.
                ReadRowsStream responseRead = bigtableClient.ReadRows(tableNameClient, filter: RowFilters.CellsPerRowLimit(1));

                Task printRead = PrintReadRowsAsync(responseRead);
                printRead.Wait();

                // Clean up. Delete the table.
                Console.WriteLine($"Delete table: {tableId}");

                bigtableTableAdminClient.DeleteTable(name: new Google.Cloud.Bigtable.Admin.V2.TableName(projectId, instanceId, tableId));
                if (!TableExist(bigtableTableAdminClient))
                {
                    Console.WriteLine($"Table: {tableId} deleted succsessfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while running HelloWorld {ex.Message}");
            }
        }

        private static bool TableExist(BigtableTableAdminClient bigtableTableAdminClient)
        {
            var tables = bigtableTableAdminClient.ListTables(new InstanceName(projectId, instanceId));
            return tables.Any(x => x.TableName.TableId == tableId);
        }

        // Builds a <see cref="Mutation"/> for <see cref="MutateRowRequest"/> or an <see cref="MutateRowsRequest.Types.Entry"/>
        private static Mutation MutationBuilder(int greetingNumber) =>
            Mutations.SetCell(columnFamily, columnName, s_greetings[greetingNumber], new BigtableVersion(DateTime.UtcNow));

        private static async Task PrintReadRowsAsync(ReadRowsStream responseRead)
        {
            await responseRead.ForEachAsync(row =>
            {
                Console.WriteLine($"\tRow key: {row.Key.ToStringUtf8()} " +
                                  $"  -- Value: {row.Families[0].Columns[0].Cells[0].Value.ToStringUtf8(),-16} " +
                                  $"  -- Time Stamp: {row.Families[0].Columns[0].Cells[0].TimestampMicros}");
            });
        }

        public static int Main(string[] args)
        {
            // Instantiates a BigtableTableAdminclient used to create a table.
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();

            if (projectId == "YOUR-PROJECT" + "-ID")
            {
                Console.WriteLine("Edit HelloWorld.cs and replace YOUR-PROJECT-ID with your project id.");
                return -1;
            }
            if (instanceId == "YOUR-INSTANCE" + "-ID")
            {
                Console.WriteLine("Edit HelloWorld.cs and replace YOUR-INSTANCE-ID with your instance id.");
                return -1;
            }

            DoHelloWorld();
            return 0;
        }
    }
}
