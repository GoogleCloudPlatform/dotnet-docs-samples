// Copyright 2022 Google Inc.
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

// [START spanner_postgresql_jsonb_add_column]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class AddJsonbColumnAsyncPostgresSample
{
    public async Task AddJsonbColumnAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        string alterStatement = $"ALTER TABLE VenueDetails ADD COLUMN Details JSONB";

        using var connection = new SpannerConnection(connectionString);
        using var ddlCmd = connection.CreateDdlCommand(alterStatement);
        await ddlCmd.ExecuteNonQueryAsync();
        Console.WriteLine($"Added the JSONB column named Details to VenueDetails table.");
    }
}
// [END spanner_postgresql_jsonb_add_column]
