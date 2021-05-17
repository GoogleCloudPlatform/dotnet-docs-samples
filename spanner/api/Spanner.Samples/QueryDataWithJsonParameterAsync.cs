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
using Newtonsoft.Json;
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
        // If you are using .NET Core 3.1 or later, you can use System.Text.Json for serialization instead.
        var jsonValue = JsonConvert.SerializeObject(new { rating = 9 });
        // Get all venues with rating 9.
        using var cmd = connection.CreateSelectCommand(
            @"SELECT VenueId, VenueDetails
              FROM Venues
              WHERE JSON_VALUE(VenueDetails, '$.rating') = JSON_VALUE(@details, '$.rating')",
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
