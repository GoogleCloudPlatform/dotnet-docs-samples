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

// [START spanner_query_with_query_options]

using Google.Cloud.Spanner.Data;
using System;
using System.Threading.Tasks;

public class RunCommandWithQueryOptionsAsyncSample
{
    public async Task RunCommandWithQueryOptionsAsync(string projectId, string instanceId, string databaseId)
    {
        var builder = new SpannerConnectionStringBuilder
        {
            DataSource = $"projects/{projectId}/instances/{instanceId}/databases/{databaseId}"
        };
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(builder))
        {
            var cmd = connection.CreateSelectCommand("SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
            // Set query options just for this command.
            cmd.QueryOptions = QueryOptions.Empty.WithOptimizerVersion("1");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine("SingerId : " + reader.GetFieldValue<string>("SingerId")
                    + " AlbumId : " + reader.GetFieldValue<string>("AlbumId")
                    + " AlbumTitle : " + reader.GetFieldValue<string>("AlbumTitle"));
                }
            }
        }
    }
}
// [END spanner_query_with_query_options]
