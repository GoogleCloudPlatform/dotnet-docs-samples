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
    public class Album
    {
        public int SingerId { get; set; }
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
    }

    public async Task<int> DeleteDataAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        var albums = new List<Album>
        {
            new Album { SingerId = 2, AlbumId = 1, AlbumTitle = "Green" },
            new Album { SingerId = 2, AlbumId = 3, AlbumTitle = "Terrified" },
        };

        int rowCount = 0;
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            // Delete individual rows from the Albums table.
            await Task.WhenAll(albums.Select(async album =>
            {
                var cmd = connection.CreateDeleteCommand("Albums", new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.Int64, album.SingerId },
                    { "AlbumId", SpannerDbType.Int64, album.AlbumId }
                });
                rowCount += await cmd.ExecuteNonQueryAsync();
            }));
            Console.WriteLine("Deleted individual rows in Albums.");

            // Delete a range of rows from the Singers table where the column key is >=3 and <5.
            var cmd = connection.CreateDmlCommand("DELETE FROM Singers WHERE SingerId >= 3 AND SingerId < 5");
            rowCount += await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"{rowCount} row(s) deleted from Singers.");

            // Delete remaining Singers rows, which will also delete the remaining
            // Albums rows since it was defined with ON DELETE CASCADE.
            cmd = connection.CreateDmlCommand("DELETE FROM Singers WHERE true");
            rowCount += await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"{rowCount} row(s) deleted from Singers.");
        }
        return rowCount;
    }
}
// [END spanner_delete_data]
