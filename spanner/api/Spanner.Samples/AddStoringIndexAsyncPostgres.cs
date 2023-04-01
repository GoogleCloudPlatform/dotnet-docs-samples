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

// [START spanner_postgresql_create_storing_index]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class AddStoringIndexAsyncPostgresSample
{
    public async Task AddStoringIndexAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        string createIndexStatement = "CREATE INDEX SingersBySingerName ON Singers(FirstName) INCLUDE(LastName, SingerInfo)";

        using var connection = new SpannerConnection(connectionString);
        using var createCmd = connection.CreateDdlCommand(createIndexStatement);
        await createCmd.ExecuteNonQueryAsync();
        Console.WriteLine("Added the SingersBySingerName index on Singers table.");
    }
}
// [END spanner_postgresql_create_storing_index]
