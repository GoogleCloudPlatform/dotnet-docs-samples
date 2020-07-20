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

// [START spanner_query_data_with_index]
// [START spanner_read_data_with_index]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryDataWithIndexAsyncSample
{
    public async Task<List<Album>> QueryDataWithIndexAsync(string projectId, string instanceId, string databaseId, string startTitle = "Aardvark", string endTitle = "Goo")
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            var cmd = connection.CreateSelectCommand(
                "SELECT AlbumId, AlbumTitle, MarketingBudget FROM Albums@ "
                + "{FORCE_INDEX=AlbumsByAlbumTitle} "
                + $"WHERE AlbumTitle >= @startTitle "
                + $"AND AlbumTitle < @endTitle",
                new SpannerParameterCollection { { "startTitle", SpannerDbType.String }, { "endTitle", SpannerDbType.String } });
            cmd.Parameters["startTitle"].Value = startTitle;
            cmd.Parameters["endTitle"].Value = endTitle;
            var albums = new List<Album>();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var marketingBudget = reader.IsDBNull(reader.GetOrdinal("MarketingBudget")) ? 0 : reader.GetFieldValue<long>("MarketingBudget");
                    var albumId = reader.GetFieldValue<int>("AlbumId");
                    var albumTitle = reader.GetFieldValue<string>("AlbumTitle");
                    albums.Add(new Album
                    {
                        AlbumId = albumId,
                        AlbumTitle = albumTitle,
                        MarketingBudget = marketingBudget
                    });
                }
            }
            return albums;
        }
    }
}
// [END spanner_read_data_with_index]
// [END spanner_query_data_with_index]
