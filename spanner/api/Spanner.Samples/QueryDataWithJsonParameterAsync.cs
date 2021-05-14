// Copyright 2021 Google Inc.
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

// [START spanner_query_with_json_parameter]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryDataWithJsonParameterAsyncSample
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string VenueDetails { get; set; }
    }

    public async Task<List<Venue>> QueryDataWithJsonParameterAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        var jsonValue = @"{
                            ""rating"": ""9"",
                            ""description"": ""This is a nice place.""
                          }";
        // Get all venues with description 'This is a nice place'.
        using var cmd = connection.CreateSelectCommand(
            @"SELECT VenueId, VenueDetails
              FROM Venues
              WHERE JSON_VALUE(VenueDetails, '$.description') = JSON_VALUE(@details, '$.description')",
            new SpannerParameterCollection
            {
                { "details", SpannerDbType.Json, jsonValue }
            });

        var venues = new List<Venue>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            venues.Add(new Venue
            {
                VenueId = reader.GetFieldValue<int>("VenueId"),
                VenueDetails = reader.GetFieldValue<string>("VenueDetails")
            });
        }
        return venues;
    }
}
// [END spanner_query_with_json_parameter]
