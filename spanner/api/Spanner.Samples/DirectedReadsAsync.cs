// Copyright 2024 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License").
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
//
// https://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and 
// limitations under the License.

// [START spanner_directed_read]

using Google.Cloud.Spanner.Data;
using Google.Cloud.Spanner.V1;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DirectedReadsAsyncSample
{
    public class Album
    {
        public int SingerId { get; set; }
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
    }

    public async Task<List<Album>> DirectedReadsAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);

        using var cmd = connection.CreateSelectCommand("SELECT SingerId, AlbumId, AlbumTitle FROM Albums");
        // Set directed read options on a query or read command.
        cmd.DirectedReadOptions = new DirectedReadOptions
        {
            IncludeReplicas = new DirectedReadOptions.Types.IncludeReplicas
            {
                AutoFailoverDisabled = true,
                ReplicaSelections =
                {
                    new DirectedReadOptions.Types.ReplicaSelection
                    {
                        Location = "us-central1",
                        Type = DirectedReadOptions.Types.ReplicaSelection.Types.Type.ReadOnly
                    }
                }
            }
        };

        var albums = new List<Album>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            albums.Add(new Album
            {
                AlbumId = reader.GetFieldValue<int>("AlbumId"),
                SingerId = reader.GetFieldValue<int>("SingerId"),
                AlbumTitle = reader.GetFieldValue<string>("AlbumTitle")
            });
        }
        return albums;
    }
}
// [END spanner_directed_read]
