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

public class InsertSampleDataAsyncSample
{
    public async Task InsertSampleDataAsync(string projectId, string instanceId, string databaseId)
    {
        const int firstSingerId = 1;
        const int secondSingerId = 2;
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        List<Singer> singers = new List<Singer>
        {
            new Singer { SingerId = firstSingerId, FirstName = "Marc", LastName = "Richards" },
            new Singer { SingerId = secondSingerId, FirstName = "Catalina", LastName = "Smith" },
            new Singer { SingerId = 3, FirstName = "Alice", LastName = "Trentor" },
            new Singer { SingerId = 4, FirstName = "Lea", LastName = "Martin" },
            new Singer { SingerId = 5, FirstName = "David", LastName = "Lomond" },
        };
        List<Album> albums = new List<Album>
        {
            new Album { SingerId = firstSingerId, AlbumId = 1, AlbumTitle = "Total Junk" },
            new Album { SingerId = firstSingerId, AlbumId = 2, AlbumTitle = "Go, Go, Go" },
            new Album { SingerId = secondSingerId, AlbumId = 1, AlbumTitle = "Green" },
            new Album { SingerId = secondSingerId, AlbumId = 2, AlbumTitle = "Forever Hold your Peace" },
            new Album { SingerId = secondSingerId, AlbumId = 3, AlbumTitle = "Terrified" },
        };
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            // Insert rows into the Singers table.
            var cmd = connection.CreateInsertCommand("Singers", new SpannerParameterCollection
            {
                { "SingerId", SpannerDbType.Int64 },
                { "FirstName", SpannerDbType.String },
                { "LastName", SpannerDbType.String }
            });
            await Task.WhenAll(singers.Select(singer =>
            {
                cmd.Parameters["SingerId"].Value = singer.SingerId;
                cmd.Parameters["FirstName"].Value = singer.FirstName;
                cmd.Parameters["LastName"].Value = singer.LastName;
                return cmd.ExecuteNonQueryAsync();
            }));

            // Insert rows into the Albums table.
            cmd = connection.CreateInsertCommand("Albums",
                new SpannerParameterCollection
                {
                    { "SingerId", SpannerDbType.Int64 },
                    { "AlbumId", SpannerDbType.Int64 },
                    { "AlbumTitle", SpannerDbType.String }
                });

            await Task.WhenAll(albums.Select(album =>
            {
                cmd.Parameters["SingerId"].Value = album.SingerId;
                cmd.Parameters["AlbumId"].Value = album.AlbumId;
                cmd.Parameters["AlbumTitle"].Value = album.AlbumTitle;
                return cmd.ExecuteNonQueryAsync();
            }));
            Console.WriteLine("Inserted data.");
        }
    }
}
// [END spanner_insert_data]
