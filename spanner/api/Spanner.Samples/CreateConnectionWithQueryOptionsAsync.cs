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

// [START spanner_create_client_with_query_options]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CreateConnectionWithQueryOptionsAsyncSample
{
    public async Task<List<Album>> CreateConnectionWithQueryOptionsAsync(string projectId, string instanceId, string databaseId)
    {
        var builder = new SpannerConnectionStringBuilder
        {
            DataSource = $"projects/{projectId}/instances/{instanceId}/databases/{databaseId}"
        };

        var albums = new List<Album>();
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(builder))
        {
            // Set query options on the connection.
            connection.QueryOptions = QueryOptions.Empty.WithOptimizerVersion("1");
            var cmd = connection.CreateSelectCommand("SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    albums.Add(new Album
                    {
                        SingerId = reader.GetFieldValue<int>("SingerId"),
                        AlbumId = reader.GetFieldValue<int>("AlbumId"),
                        AlbumTitle = reader.GetFieldValue<string>("AlbumTitle")
                    });
                }
            }
        }
        return albums;
    }
}
// [END spanner_create_client_with_query_options]
