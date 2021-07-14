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

// [START spanner_update_data_with_json_column]

using Google.Cloud.Spanner.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UpdateDataWithJsonAsyncSample
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string VenueDetails { get; set; }
    }

    public async Task UpdateDataWithJsonAsync(string projectId, string instanceId, string databaseId)
    {
        List<Venue> venues = new List<Venue>
        {
            // If you are using .NET Core 3.1 or later, you can use System.Text.Json for serialization instead.
            new Venue
            {
                VenueId = 19,
                VenueDetails = JsonConvert.SerializeObject(new
                {
                    rating = 9,
                    open = true,
                })
            },
            new Venue
            {
                VenueId = 4,
                VenueDetails = JsonConvert.SerializeObject(new object[]
                {
                    new
                    {
                        name = "room 1",
                        open = true,
                    },
                    new
                    {
                        name = "room 2",
                        open = false,
                    },
                })
            },
            new Venue
            {
                VenueId = 42,
                VenueDetails = JsonConvert.SerializeObject(new
                {
                    name = "Central Park",
                    open = new
                    {
                        Monday = true,
                        Tuesday = false,
                    },
                    tags = new string[] {"large", "airy" },
                }),
            },
        };
        // Create connection to Cloud Spanner.
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        await Task.WhenAll(venues.Select(venue =>
        {
            // Update rows in the Venues table.
            using var cmd = connection.CreateUpdateCommand("Venues", new SpannerParameterCollection
            {
                    { "VenueId", SpannerDbType.Int64, venue.VenueId },
                    { "VenueDetails", SpannerDbType.Json, venue.VenueDetails }
            });
            return cmd.ExecuteNonQueryAsync();
        }));

        Console.WriteLine("Data updated.");
    }
}
// [END spanner_update_data_with_json_column]
