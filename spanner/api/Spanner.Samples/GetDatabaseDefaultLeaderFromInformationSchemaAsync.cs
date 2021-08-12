// Copyright 2021 Google Inc.
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

// [START spanner_query_information_schema_database_options]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class GetDatabaseDefaultLeaderFromInformationSchemaAsyncSample
{
    public async Task<string> GetDatabaseDefaultLeaderFromInformationSchemaAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        var cmd =
            connection.CreateSelectCommand(
            @"SELECT 
                s.OPTION_NAME,
                s.OPTION_VALUE
            FROM
                INFORMATION_SCHEMA.DATABASE_OPTIONS s
            WHERE
                s.OPTION_NAME = 'default_leader'");

        var defaultLeader = string.Empty;

        Console.WriteLine($"Default leader for {databaseId}:");
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            defaultLeader = reader.GetFieldValue<string>("OPTION_VALUE");
            Console.WriteLine(defaultLeader);
        }

        return defaultLeader;
    }
}
// [END spanner_query_information_schema_database_options]
