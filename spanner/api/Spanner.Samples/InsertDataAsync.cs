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

// [START spanner_insert_data]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class InsertDataAsyncSample
{
    public class Singer
    {
        public int SingerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Album
    {
        public int SingerId { get; set; }
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
    }

    public async Task InsertDataAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        List<Singer> singers = new List<Singer>
        {
            new Singer { SingerId = 1, FirstName = "Marc", LastName = "Richards" },
            new Singer { SingerId = 2, FirstName = "Catalina", LastName = "Smith" },
            new Singer { SingerId = 3, FirstName = "Alice", LastName = "Trentor" },
            new Singer { SingerId = 4, FirstName = "Lea", LastName = "Martin" },
            new Singer { SingerId = 5, FirstName = "David", LastName = "Lomond" },
        };
        List<Album> albums = new List<Album>
        {
            new Album { SingerId = 1, AlbumId = 1, AlbumTitle = "Total Junk" },
            new Album { SingerId = 1, AlbumId = 2, AlbumTitle = "Go, Go, Go" },
            new Album { SingerId = 2, AlbumId = 1, AlbumTitle = "Green" },
            new Album { SingerId = 2, AlbumId = 2, AlbumTitle = "Forever Hold your Peace" },
            new Album { SingerId = 2, AlbumId = 3, AlbumTitle = "Terrified" },
        };
        // Create connection to Cloud Spanner.
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        await Task.WhenAll(singers.Select(singer =>
        {
            // Insert rows into the Singers table.
            using var cmd = connection.CreateInsertCommand("Singers", new SpannerParameterCollection
            {
                    { "SingerId", SpannerDbType.Int64, singer.SingerId },
                    { "FirstName", SpannerDbType.String, singer.FirstName },
                    { "LastName", SpannerDbType.String, singer.LastName }
            });
            return cmd.ExecuteNonQueryAsync();
        }));

        await Task.WhenAll(albums.Select(album =>
        {
            // Insert rows into the Albums table.
            using var cmd = connection.CreateInsertCommand("Albums", new SpannerParameterCollection
            {
                    { "SingerId", SpannerDbType.Int64, album.SingerId },
                    { "AlbumId", SpannerDbType.Int64, album.AlbumId },
                    { "AlbumTitle", SpannerDbType.String,album.AlbumTitle }
            });
            return cmd.ExecuteNonQueryAsync();
        }));
        Console.WriteLine("Data inserted.");
    }
}
// [END spanner_insert_data]
