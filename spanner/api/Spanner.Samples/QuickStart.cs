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

// [START spanner_quickstart]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class QuickStartSample
{
    public async Task QuickStart(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            // Execute a simple SQL statement.
            var cmd = connection.CreateSelectCommand(@"SELECT ""Hello World"" as test");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine(reader.GetFieldValue<string>("test"));
                }
            }
        }
    }
}
// [END spanner_quickstart]
