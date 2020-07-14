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

// [START spanner_delete_data]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DeleteDataAsyncSample
{
    public async Task DeleteIndividualRowsAsync(string projectId, string instanceId, string databaseId)
    {
        const int singerId = 2;
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        List<Album> albums = new List<Album>
        {
            new Album { SingerId = singerId, AlbumId = 1, AlbumTitle = "Green" },
            new Album { SingerId = singerId, AlbumId = 3, AlbumTitle = "Terrified" },
        };
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            // Delete individual rows from the UpcomingAlbums table.
            await Task.WhenAll(albums.Select(album =>
            {
                var cmd = connection.CreateDeleteCommand("UpcomingAlbums", new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.Int64, album.SingerId },
                    { "AlbumId", SpannerDbType.Int64, album.AlbumId }
                }
                );
                return cmd.ExecuteNonQueryAsync();
            }));
            Console.WriteLine("Deleted individual rows in UpcomingAlbums.");

            // Delete a range of rows from the UpcomingSingers table where the column key is >=3 and <5.
            var cmd = connection.CreateDmlCommand("DELETE FROM UpcomingSingers WHERE SingerId >= 3 AND SingerId < 5");
            int rowCount = await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"{rowCount} row(s) deleted from UpcomingSingers.");

            // Delete remaining UpcomingSingers rows, which will also delete the remaining
            // UpcomingAlbums rows since it was defined with ON DELETE CASCADE.
            cmd = connection.CreateDmlCommand("DELETE FROM UpcomingSingers WHERE true");
            rowCount = await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"{rowCount} row(s) deleted from UpcomingSingers.");
        }
    }
}
// [END spanner_insert_data]
