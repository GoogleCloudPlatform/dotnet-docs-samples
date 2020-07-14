// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START spanner_list_database_tables]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ListDatabaseTablesAsyncSample
{
    public async Task<List<string>> ListDatabaseTablesAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Create connection to Cloud Spanner.
        List<string> tables = new List<string>();
        using (var connection = new SpannerConnection(connectionString))
        {
            var cmd = connection.CreateSelectCommand(
                "SELECT t.table_name FROM information_schema.tables AS t "
                + "WHERE t.table_catalog = '' and t.table_schema = ''");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var tableName = reader.GetFieldValue<string>("table_name");
                    tables.Add(tableName);
                    Console.WriteLine(tableName);
                }
            }
        }
        return tables;
    }
}
// [END spanner_list_database_tables]
