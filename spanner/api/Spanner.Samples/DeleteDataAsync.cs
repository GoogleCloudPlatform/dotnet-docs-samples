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
    public async Task<int> DeleteIndividualRowsAsync(string projectId, string instanceId, string databaseId)
    {
        const int singerId = 2;
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        // Create Sample Tables.
        using (var connection = new SpannerConnection(connectionString))
        {
            // Define create table statement for table #1.
            string createTableStatement =
           @"CREATE TABLE UpcomingSingers (
                     SingerId INT64 NOT NULL,
                     FirstName    STRING(1024),
                     LastName STRING(1024),
                     ComposerInfo   BYTES(MAX)
                 ) PRIMARY KEY (SingerId)";
            // Make the request.
            var cmd = connection.CreateDdlCommand(createTableStatement);
            await cmd.ExecuteNonQueryAsync();
            // Define create table statement for table #2.
            createTableStatement =
            @"CREATE TABLE UpcomingAlbums (
                     SingerId     INT64 NOT NULL,
                     AlbumId      INT64 NOT NULL,
                     AlbumTitle   STRING(MAX)
                 ) PRIMARY KEY (SingerId, AlbumId),
                 INTERLEAVE IN PARENT UpcomingSingers ON DELETE CASCADE";
            // Make the request.
            cmd = connection.CreateDdlCommand(createTableStatement);
            await cmd.ExecuteNonQueryAsync();
        }

        // Insert Data into sample tables.
        const int firstSingerId = 1;
        const int secondSingerId = 2;
        List<Singer> singers = new List<Singer>
            {
                new Singer { SingerId = firstSingerId, FirstName = "Marc",
                    LastName = "Richards" },
                new Singer { SingerId = secondSingerId, FirstName = "Catalina",
                    LastName = "Smith" },
                new Singer { SingerId = 3, FirstName = "Alice",
                    LastName = "Trentor" },
                new Singer { SingerId = 4, FirstName = "Lea",
                    LastName = "Martin" },
                new Singer { SingerId = 5, FirstName = "David",
                    LastName = "Lomond" },
            };
        List<Album> albums = new List<Album>
            {
                new Album { SingerId = firstSingerId, AlbumId = 1,
                    AlbumTitle = "Total Junk" },
                new Album { SingerId = firstSingerId, AlbumId = 2,
                    AlbumTitle = "Go, Go, Go" },
                new Album { SingerId = secondSingerId, AlbumId = 1,
                    AlbumTitle = "Green" },
                new Album { SingerId = secondSingerId, AlbumId = 2,
                    AlbumTitle = "Forever Hold your Peace" },
                new Album { SingerId = secondSingerId, AlbumId = 3,
                    AlbumTitle = "Terrified" },
            };
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            // Insert rows into the Singers table.
            var cmd = connection.CreateInsertCommand("UpcomingSingers",
                new SpannerParameterCollection
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
            cmd = connection.CreateInsertCommand("UpcomingAlbums",
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

        albums = new List<Album>
        {
            new Album { SingerId = singerId, AlbumId = 1, AlbumTitle = "Green" },
            new Album { SingerId = singerId, AlbumId = 3, AlbumTitle = "Terrified" },
        };
        int rowCount;
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            // Delete individual rows from the UpcomingAlbums table.
            var rowCountArray = await Task.WhenAll(albums.Select(album =>
              {
                  var cmd = connection.CreateDeleteCommand("UpcomingAlbums", new SpannerParameterCollection
                  {
                    { "SingerId", SpannerDbType.Int64, album.SingerId },
                    { "AlbumId", SpannerDbType.Int64, album.AlbumId }
                  });
                  return cmd.ExecuteNonQueryAsync();
              }));
            Console.WriteLine("Deleted individual rows in UpcomingAlbums.");

            // Delete a range of rows from the UpcomingSingers table where the column key is >=3 and <5.
            var cmd = connection.CreateDmlCommand("DELETE FROM UpcomingSingers WHERE SingerId >= 3 AND SingerId < 5");
            rowCount = rowCountArray.Sum();
            rowCount += await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"{rowCount} row(s) deleted from UpcomingSingers.");

            // Delete remaining UpcomingSingers rows, which will also delete the remaining
            // UpcomingAlbums rows since it was defined with ON DELETE CASCADE.
            cmd = connection.CreateDmlCommand("DELETE FROM UpcomingSingers WHERE true");
            rowCount += await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"{rowCount} row(s) deleted from UpcomingSingers.");
        }

        // Delete Sample Tables

        using (var connection = new SpannerConnection(connectionString))
        {
            foreach (var table in new List<string> { "UpcomingAlbums", "UpcomingSingers" })
            {
                try
                {
                    await connection.CreateDdlCommand($"DROP TABLE {table}").ExecuteNonQueryAsync();
                }
                catch (SpannerException e) when (e.ErrorCode == ErrorCode.NotFound)
                {
                    // Table does not exist.  Not a problem.
                }
            }
        }
        return rowCount;
    }
}
// [END spanner_insert_data]
