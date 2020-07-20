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

// [START spanner_query_data_with_new_column]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryNewColumnAsyncSample
{
    public async Task<List<Album>> QueryNewColumnAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        var albums = new List<Album>();
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            var cmd = connection.CreateSelectCommand("SELECT * FROM Albums");
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    long? budget = null;
                    if (reader["MarketingBudget"] != DBNull.Value)
                    {
                        budget = reader.GetFieldValue<long>("MarketingBudget");
                    }
                    var singerId = reader.GetFieldValue<int>("SingerId");
                    var albumId = reader.GetFieldValue<int>("AlbumId");
                    albums.Add(new Album
                    {
                        SingerId = singerId,
                        AlbumId = albumId,
                        MarketingBudget = budget == null ? null : budget
                    }); ;
                }
            }
        }
        return albums;
    }
}
// [END spanner_query_data_with_new_column]
