// Copyright 2023 Google Inc.
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

// [START spanner_read_data_with_database_role]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ReadDataWithDatabaseRoleAsyncSample
{
    public class Singer
    {
        public int SingerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public async Task<List<Singer>> ReadDataWithDatabaseRoleAsync(string projectId, string instanceId, string databaseId, string databaseRole)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        string tableName = "Singers";

        var spannerConnectionStringBuilder = new SpannerConnectionStringBuilder
        {
            ConnectionString = connectionString,
            DatabaseRole = databaseRole
        };
        using var connection = new SpannerConnection(spannerConnectionStringBuilder);
        var createSelectCmd = connection.CreateSelectCommand($"SELECT * FROM {tableName}");
        using var reader = await createSelectCmd.ExecuteReaderAsync();
        var singers = new List<Singer>();
        while (await reader.ReadAsync())
        {
            singers.Add(new Singer
            {
                SingerId = reader.GetFieldValue<int>("SingerId"),
                FirstName = reader.GetFieldValue<string>("FirstName"),
                LastName = reader.GetFieldValue<string>("LastName"),
            });
        }
        return singers;
    }
}
// [END spanner_read_data_with_database_role]
