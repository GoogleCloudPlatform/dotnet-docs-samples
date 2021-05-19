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

// [START spanner_query_with_array_parameter]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryWithArrayAsyncSample
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public List<DateTime> AvailableDates { get; set; }
    }

    public async Task<List<Venue>> QueryWithArrayAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Initialize a list of dates to use for querying.
        var exampleList = new List<DateTime>
        {
            DateTime.Parse("2020-10-01"),
            DateTime.Parse("2020-11-01")
        };

        using var connection = new SpannerConnection(connectionString);
        var cmd = connection.CreateSelectCommand(
            "SELECT VenueId, VenueName, AvailableDate FROM Venues v, "
            + "UNNEST(v.AvailableDates) as AvailableDate "
            + "WHERE AvailableDate in UNNEST(@ExampleArray)");
        cmd.Parameters.Add("ExampleArray", SpannerDbType.ArrayOf(SpannerDbType.Date), exampleList);

        var venues = new List<Venue>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            venues.Add(new Venue
            {
                VenueId = reader.GetFieldValue<int>("VenueId"),
                VenueName = reader.GetFieldValue<string>("VenueName"),
                AvailableDates = new List<DateTime> { reader.GetFieldValue<DateTime>("AvailableDate") }
            });
        }
        return venues;
    }
}
// [END spanner_query_with_array_parameter]
