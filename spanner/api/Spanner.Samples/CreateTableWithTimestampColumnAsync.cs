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

// [START spanner_create_table_with_timestamp_column]

using Google.Cloud.Spanner.Data;
using System.Threading.Tasks;

public class CreateTableWithTimestampColumnAsyncSample
{
    public async Task CreateTableWithTimestampColumnAsync(string projectId, string instanceId, string databaseId)
    {
        // Initialize request connection string for database creation.
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using (var connection = new SpannerConnection(connectionString))
        {
            // Define create table statement for table with
            // commit timestamp column.
            string createTableStatement =
            @"CREATE TABLE Performances (
                    SingerId       INT64 NOT NULL,
                    VenueId        INT64 NOT NULL,
                    EventDate      Date,
                    Revenue        INT64,
                    LastUpdateTime TIMESTAMP NOT NULL OPTIONS (allow_commit_timestamp=true)
                ) PRIMARY KEY (SingerId, VenueId, EventDate),
                    INTERLEAVE IN PARENT Singers ON DELETE CASCADE";
            // Make the request.
            var cmd = connection.CreateDdlCommand(createTableStatement);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
// [END spanner_create_table_with_timestamp_column]
