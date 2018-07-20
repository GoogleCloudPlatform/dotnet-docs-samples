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
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Google.Api.Gax;
using Google.LongRunning;
using Google.Cloud.Bigtable.Admin.V2;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GoogleCloudSamples.Bigtable
{
    [Verb("createTable", HelpText = "Creates a table in the Instance.")]
    class CreateTableOptions
    {
        [Value(0, HelpText = "The tableId to create.", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("listTables", HelpText = "Lists tables in the Instance.")]
    class ListTablesOptions
    {
    }

    [Verb("getTable", HelpText = "Gets information about a table.")]
    class GetTableOptions
    {
        [Value(0, HelpText = "The tableId to get.", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("createMaxAgeFamily", HelpText = "Creates a column family with max age GC rule.")]
    class CreateMaxAgeFamilyOptions
    {
        [Value(0, HelpText = "The tableId to use.", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("createMaxVersionsFamily", HelpText = "Creates a column family with max versions GC rule.")]
    class CreateMaxVersionsFamilyOptions
    {
        [Value(0, HelpText = "The tableId to use.", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("createUnionFamily", HelpText = "Creates a column family with union GC rule.")]
    class CreateUnionFamilyOptions
    {
        [Value(0, HelpText = "The tableId to use.", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("createIntersectionFamily", HelpText = "Creates a column family with intersection GC rule.")]
    class CreateIntersectionFamilyOptions
    {
        [Value(0, HelpText = "The tableId to use.", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("createNestedFamily", HelpText = "Creates a column family with nested GC rules.")]
    class CreateNestedFamilyOptions
    {
        [Value(0, HelpText = "The tableId to use.", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("updateFamily", HelpText = "Update the column family metadata to update the GC rule.")]
    class UpdateFamilyOptions
    {
        [Value(0, HelpText = "The table to use", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("deleteFamily", HelpText = "Deletes a columnFamily.")]
    class DeleteFamilyOptions
    {
        [Value(0, HelpText = "The table to use.", Required = true)]
        public string tableId { get; set; }
    }

    [Verb("deleteTable", HelpText = "Deletes a table from the Instance.")]
    class DeleteTableOptions
    {
        [Value(0, HelpText = "The tableId to delete.", Required = true)]
        public string tableId { get; set; }
    }

    public class TableAdmin
    {
        private const string projectId = "YOUR-PROJECT-ID";

        private const string instanceId = "YOUR-INSTANCE-ID";

        private static readonly InstanceName s_instanceName = new InstanceName(projectId, instanceId);

        private static object CreateTable(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Creating table");
            // [START bigtable_create_table]
            // Creates table if doesn't exist.
            // Initialize request argument(s).
            // Table to create
            Table table = new Table
            {
                Granularity = Table.Types.TimestampGranularity.Millis
            };

            CreateTableRequest request = new CreateTableRequest
            {
                ParentAsInstanceName = s_instanceName,
                Table = table,
                TableId = tableId
            };

            try
            {
                if (!TableExist(bigtableTableAdminClient, tableId))
                {
                    // Make the request.
                    Table response = bigtableTableAdminClient.CreateTable(request);
                    // [END bigtable_create_table]
                    // Print table information.
                    GetTable(tableId);
                    // [START bigtable_create_table]
                }
                else
                {
                    Console.WriteLine("Table exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating table {ex.Message}");
            }
            // [END bigtable_create_table]

            return 0;
        }

        private static object ListTables()
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Listing tables");
            // [START bigtable_list_tables]
            // Lists tables in intance.
            // Initialize request argument(s).
            ListTablesRequest request = new ListTablesRequest
            {
                ParentAsInstanceName = s_instanceName
            };
            try
            {
                // Make the request.
                PagedEnumerable<ListTablesResponse, Table> response = bigtableTableAdminClient.ListTables(request);

                // [END bigtable_list_tables]
                // Iterate over all response items, lazily performing RPCs as required
                foreach (Table table in response)
                {
                    // Print table information.
                    PrintTableInfo(table);
                }
                // [START bigtable_list_tables]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing tables {ex.Message}");
            }
            // [END bigtable_list_tables]
            return 0;
        }

        private static object GetTable(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Getting table");
            // [START bigtable_get_table]
            // Getting information about a table.
            // Initialize request argument(s).
            // Table to get.
            TableName tableName = new TableName(projectId, instanceId, tableId);
            GetTableRequest request = new GetTableRequest
            {
                TableName = tableName
            };
            try
            {
                // Make the request
                Table table = bigtableTableAdminClient.GetTable(request);
                // [END bigtable_get_table]
                // Print table information.
                PrintTableInfo(table);
                // [START bigtable_get_table]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting table {ex.Message}");
            }
            // [END bigtable_get_table]
            return 0;
        }

        private static object CreateMaxAgeFamily(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Creating column family cf1 with max age GC rule...");
            // [START bigtable_create_family_gc_max_age]
            // Create a column family with GC policy : maximum age
            // where age = current time minus cell timestamp
            // Initialize request argument(s).
            // Define the GC rule to retain data with max age of 5 days
            GcRule MaxAgeRule = new GcRule { MaxAge = Duration.FromTimeSpan(TimeSpan.FromDays(5.0)) };

            // Column family to create
            ColumnFamily columnFamily = new ColumnFamily { GcRule = MaxAgeRule };

            TableName tableName = new TableName(projectId, instanceId, tableId);

            // Modification to create column family
            ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
            {
                Create = columnFamily,
                Id = "cf1"
            };

            ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
            {
                TableName = tableName,
                Modifications = { modification }
            };
            try
            {
                // Make the request
                Table response = bigtableTableAdminClient.ModifyColumnFamilies(request);
                Console.WriteLine("Created column family");
                // [END bigtable_create_bigtableTableAdminClient]
                // Print table information.
                GetTable(tableId);
                // [START bigtable_create_bigtableTableAdminClient]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating column family {ex.Message}");
            }
            // [END bigtable_create_family_gc_max_age]
            return 0;
        }

        private static object CreateMaxVersionsFamily(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Creating column family cf2 with max versions GC rule...");
            // [START bigtable_create_family_gc_max_versions]
            // Create a column family with GC policy : most recent N versions
            // where 1 = most recent version
            // Initialize request argument(s).
            // Define the GC policy to retain only the most recent 2 versions
            GcRule maxVersionsRule = new GcRule { MaxNumVersions = 2 };

            // Column family to create
            ColumnFamily columnFamily = new ColumnFamily { GcRule = maxVersionsRule };

            TableName tableName = new TableName(projectId, instanceId, tableId);

            // Modification to create column family
            ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
            {
                Create = columnFamily,
                Id = "cf2"
            };

            ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
            {
                TableName = tableName,
                Modifications = { modification }
            };
            try
            {
                // Make the request
                Table response = bigtableTableAdminClient.ModifyColumnFamilies(request);
                Console.WriteLine("Created column family");
                // [END bigtable_create_family_gc_max_versions]
                // Print table information.
                GetTable(tableId);
                // [START bigtable_create_family_gc_max_versions]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating column family {ex.Message}");
            }
            // [END bigtable_create_family_gc_max_versions]
            return 0;
        }

        private static object CreateUnionFamily(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Creating column family cf3 with union GC rule...");
            // [START bigtable_create_family_gc_union]
            // Create a column family with GC policy to drop data that matches at least one condition.
            // Initialize request argument(s).
            // Define a GC rule to drop cells older than 5 days or not the most recent version.
            GcRule.Types.Union unionRule = new GcRule.Types.Union
            {
                Rules =
                {
                    new GcRule {MaxNumVersions = 1},
                    new GcRule {MaxAge = Duration.FromTimeSpan(TimeSpan.FromDays(5))}
                }
            };
            GcRule gcRule = new GcRule { Union = unionRule };

            // Column family to create
            ColumnFamily columnFamily = new ColumnFamily { GcRule = gcRule };

            TableName tableName = new TableName(projectId, instanceId, tableId);

            // Modification to create column family
            ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
            {
                Create = columnFamily,
                Id = "cf3"
            };

            ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
            {
                TableName = tableName,
                Modifications = { modification }
            };
            try
            {
                // Make the request
                Table response = bigtableTableAdminClient.ModifyColumnFamilies(request);
                Console.WriteLine("Created column family");
                // [END bigtable_create_family_gc_union]
                // Print table information.
                GetTable(tableId);
                // [START bigtable_create_family_gc_union]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating column family {ex.Message}");
            }
            // [END bigtable_create_family_gc_union]
            return 0;
        }

        private static object CreateIntersectionFamily(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Creating column family cf4 with intersect GC rule...");
            // [START bigtable_create_family_gc_intersection]
            // Create a column family with GC policy to drop data that matches all conditions.
            // Initialize request argument(s).
            // GC rule: Drop cells older than 5 days AND older than the most recent 2 versions.
            GcRule.Types.Intersection intersectionRule = new GcRule.Types.Intersection
            {
                Rules =
                {
                    new GcRule {MaxNumVersions = 2},
                    new GcRule {MaxAge = Duration.FromTimeSpan(TimeSpan.FromDays(5))}
                }
            };
            GcRule gcRule = new GcRule { Intersection = intersectionRule };

            // Column family to create
            ColumnFamily columnFamily = new ColumnFamily { GcRule = gcRule };

            TableName tableName = new TableName(projectId, instanceId, tableId);

            // Modification to create column family
            ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
            {
                Create = columnFamily,
                Id = "cf4"
            };

            ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
            {
                TableName = tableName,
                Modifications = { modification }
            };
            try
            {
                // Make the request
                Table response = bigtableTableAdminClient.ModifyColumnFamilies(request);
                Console.WriteLine("Created column family");
                // [END bigtable_create_family_gc_intersection]
                // Print table information.
                GetTable(tableId);
                // [START bigtable_create_family_gc_intersection]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating column family {ex.Message}");
            }
            // [END bigtable_create_family_gc_intersection]
            return 0;
        }

        private static object CreateNestedFamily(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Creating column family cf5 with a nested GC rule...");
            // [START bigtable_create_family_gc_nested]
            // Create a nested GC rule:
            // Drop cells that are either older than the 10 recent versions
            // OR
            // Drop cells that are older than a month AND older than the 2 recent versions.
            // Initialize request argument(s).
            GcRule.Types.Intersection intersectionRule = new GcRule.Types.Intersection
            {
                Rules =
                {
                    new GcRule {MaxNumVersions = 2},
                    new GcRule {MaxAge = Duration.FromTimeSpan(TimeSpan.FromDays(5))}
                }
            };

            GcRule.Types.Union nestedRule = new GcRule.Types.Union
            {
                Rules =
                {
                    new GcRule {MaxNumVersions = 10},
                    new GcRule {Intersection = intersectionRule}
                }
            };

            GcRule gcRule = new GcRule { Union = nestedRule };

            // Column family to create
            ColumnFamily columnFamily = new ColumnFamily { GcRule = gcRule };

            TableName tableName = new TableName(projectId, instanceId, tableId);

            // Modification to create column family
            ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
            {
                Create = columnFamily,
                Id = "cf5"
            };

            ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
            {
                TableName = tableName,
                Modifications = { modification }
            };
            try
            {
                // Make the request
                Table response = bigtableTableAdminClient.ModifyColumnFamilies(request);
                Console.WriteLine("Created column family");
                // [END bigtable_create_family_gc_nested]
                // Print table information.
                GetTable(tableId);
                // [START bigtable_create_family_gc_nested]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating column family {ex.Message}");
            }
            // [END bigtable_create_family_gc_nested]
            return 0;
        }

        private static object UpdateFamily(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Updating column family cf1 GC rule...");
            // [START bigtable_update_gc_rule]
            // Update the column family metadata to update the GC rule.
            // Initialize request argument(s).
            // Updated column family GC rule.
            GcRule maxVersionsRule = new GcRule { MaxNumVersions = 1 };

            // Column family to create
            ColumnFamily columnFamily = new ColumnFamily { GcRule = maxVersionsRule };

            TableName tableName = new TableName(projectId, instanceId, tableId);

            // Modification to update column family
            ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
            {
                Update = columnFamily,
                Id = "cf1"
            };

            ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
            {
                TableName = tableName,
                Modifications = { modification }
            };
            try
            {
                // Make the request
                Table response = bigtableTableAdminClient.ModifyColumnFamilies(request);
                Console.WriteLine("Updated column family");
                // [END bigtable_update_gc_rule]
                // Print table information.
                GetTable(tableId);
                // [START bigtable_update_gc_rule]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating column family {ex.Message}");
            }

            // [END bigtable_update_gc_rule]
            return 0;
        }

        private static object DeleteFamily(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Deleting column family cf2 GC rule...");
            // [START bigtable_delete_family]
            // Delete a column family.
            // Initialize request argument(s).
            GcRule maxVersionsRule = new GcRule { MaxNumVersions = 1 };

            TableName tableName = new TableName(projectId, instanceId, tableId);

            // Modification to update column family
            ModifyColumnFamiliesRequest.Types.Modification modification = new ModifyColumnFamiliesRequest.Types.Modification
            {
                Drop = true,
                Id = "cf2"
            };

            ModifyColumnFamiliesRequest request = new ModifyColumnFamiliesRequest
            {
                TableName = tableName,
                Modifications = { modification }
            };
            try
            {
                // Make the request
                Table response = bigtableTableAdminClient.ModifyColumnFamilies(request);
                Console.WriteLine("Deleted column family");
                // [END bigtable_delete_family]
                // Print table information.
                GetTable(tableId);
                // [START bigtable_delete_family]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting column family {ex.Message}");
            }

            // [END bigtable_delete_family]
            return 0;
        }

        private static object DeleteTable(string tableId)
        {
            // [START bigtable_create_bigtableTableAdminClient]
            BigtableTableAdminClient bigtableTableAdminClient = BigtableTableAdminClient.Create();
            // [END bigtable_create_bigtableTableAdminClient]

            Console.WriteLine("Deleting table");
            // [START bigtable_delete_table]
            // Delete the entire table.
            // Initialize request argument(s).
            // Table to delete
            TableName tableName = new TableName(projectId, instanceId, tableId);

            DeleteTableRequest request = new DeleteTableRequest
            {
                TableName = tableName
            };

            try
            {
                // Make the request.
                bigtableTableAdminClient.DeleteTable(request);
                // [END bigtable_delete_table]
                // Check if table still exists.
                if (!TableExist(bigtableTableAdminClient, tableId))
                {
                    Console.WriteLine($"Table {tableId} deleted successfully");
                }
                else
                {
                    Console.WriteLine($"Table {tableId} was not deleted");
                }
                // [START bigtable_delete_table]
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating table {ex.Message}");
                GetTable(tableId);
            }
            // [END bigtable_delete_table]

            return 0;
        }

        private static bool TableExist(BigtableTableAdminClient bigtableTableAdminClient, string tableId)
        {
            // Check if table exists
            Console.WriteLine("Checking if table exists...");

            var tables = bigtableTableAdminClient.ListTables(s_instanceName);
            return tables.Any(x => x.TableName.TableId == tableId);
        }

        private static void PrintTableInfo(Table table)
        {
            Console.WriteLine(new string('-', 50));
            Console.WriteLine("Printing table information");
            Console.WriteLine($"{"Table ID:",-30}{table.TableName.TableId}");
            foreach (KeyValuePair<string, ColumnFamily> ColumnFamily in table.ColumnFamilies)
            {
                Console.WriteLine($"{"Column family:",-30}{ColumnFamily.Key}");
                PrintGcRuleInfo(ColumnFamily.Value);
            }
        }

        private static void PrintGcRuleInfo(ColumnFamily family)
        {
            Console.WriteLine($"{"GC Rule:",-30}{family.GcRule}");
        }

        public static int Main(string[] args)
        {
            if (projectId == "YOUR-PROJECT" + "-ID")
            {
                Console.WriteLine("Edit TableAdmin.cs and replace YOUR-PROJECT-ID with your project id.");
                return -1;
            }
            if (instanceId == "YOUR-INSTANCE" + "-ID")
            {
                Console.WriteLine("Edit TableAdmin.cs and replace YOUR-INSTANCE-ID with your instance id.");
                return -1;
            }

            var verbMap = new VerbMap<object>();
            verbMap
                .Add((CreateTableOptions opts) => CreateTable(opts.tableId))
                .Add((ListTablesOptions opts) => ListTables())
                .Add((GetTableOptions opts) => GetTable(opts.tableId))
                .Add((CreateMaxAgeFamilyOptions opts) => CreateMaxAgeFamily(opts.tableId))
                .Add((CreateMaxVersionsFamilyOptions opts) => CreateMaxVersionsFamily(opts.tableId))
                .Add((CreateUnionFamilyOptions opts) => CreateUnionFamily(opts.tableId))
                .Add((CreateIntersectionFamilyOptions opts) => CreateIntersectionFamily(opts.tableId))
                .Add((CreateNestedFamilyOptions opts) => CreateNestedFamily(opts.tableId))
                .Add((UpdateFamilyOptions opts) => UpdateFamily(opts.tableId))
                .Add((DeleteFamilyOptions opts) => DeleteFamily(opts.tableId))
                .Add((DeleteTableOptions opts) => DeleteTable(opts.tableId))
                .NotParsedFunc = (err) => 1;

            return (int)verbMap.Run(args);
        }
    }
}
