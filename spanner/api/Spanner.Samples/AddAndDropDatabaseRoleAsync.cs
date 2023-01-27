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

// [START spanner_add_and_drop_database_role]

using Google.Cloud.Spanner.Data;
using System.Threading.Tasks;

public class AddAndDropDatabaseRoleAsyncSample
{
    public async Task AddDatabaseRoleAsync(string projectId, string instanceId, string databaseId, string databaseRole)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        string createRoleStatement = $"CREATE ROLE {databaseRole}";

        // Creates the given database role.
        using var connection = new SpannerConnection(connectionString);
        using var updateCmd = connection.CreateDdlCommand(createRoleStatement);
        await updateCmd.ExecuteNonQueryAsync();
    }

    public async Task DropDatabaseRoleAsync(string projectId, string instanceId, string databaseId, string databaseRole)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        string deleteRoleStatement = $"DROP ROLE {databaseRole}";

        // Drops the given database role.
        using var connection = new SpannerConnection(connectionString);
        using var updateCmd = connection.CreateDdlCommand(deleteRoleStatement);
        await updateCmd.ExecuteNonQueryAsync();
    }
}
// [END spanner_add_and_drop_database_role]