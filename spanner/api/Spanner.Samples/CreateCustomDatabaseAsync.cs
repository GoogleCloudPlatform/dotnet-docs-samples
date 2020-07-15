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

// [START spanner_create_custom_database]

using Google.Cloud.Spanner.Data;
using System.Threading.Tasks;

public class CreateCustomDatabaseAsyncSample
{
    public async Task CreateCustomDatabaseAsync(string projectId, string instanceId, string databaseId)
    {
        // Initialize request connection string for database creation.
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}";
        // Make the request.
        using (var connection = new SpannerConnection(connectionString))
        {
            string createStatement = $"CREATE DATABASE `{databaseId}`";
            var cmd = connection.CreateDdlCommand(createStatement);
            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SpannerException e) when (e.ErrorCode == ErrorCode.AlreadyExists)
            {
                // Database Already Exists.
            }
        }
    }
}
// [END spanner_create_custom_database]
