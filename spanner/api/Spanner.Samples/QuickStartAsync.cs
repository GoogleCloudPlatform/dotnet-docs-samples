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
using System.Collections.Generic;
using System.Threading.Tasks;

public class QuickStartAsyncSample
{
    public async Task<List<Singer>> QuickStartAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Create connection to Cloud Spanner.
        var singers = new List<Singer>();
        using (var connection = new SpannerConnection(connectionString))
        {
            // Execute a simple SQL statement.
            var cmd = connection.CreateSelectCommand(@"SELECT * from Singers");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var singerId = reader.GetFieldValue<int>("SingerId");
                    var firstName = reader.GetFieldValue<string>("FirstName");
                    var lastName = reader.GetFieldValue<string>("LastName");
                    singers.Add(new Singer
                    {
                        SingerId = singerId,
                        FirstName = firstName,
                        LastName = lastName
                    });
                }
            }
        }
        return singers;
    }
}
// [END spanner_quickstart]
