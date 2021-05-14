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
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        List<Venue> venues = new List<Venue>
        {
            new Venue { VenueId = 19, VenueDetails = @"{
                ""rating"": ""9"",
                ""description"": ""This is a nice place.""
            }" },
            new Venue { VenueId = 4, VenueDetails = @"[
                {
                    ""wing1"": { 
                        ""description"": ""the first wing"",
                        ""size"": ""5""
                    }
                },
                {
                    ""wing2"": {
                        ""description"": ""the second wing"",
                        ""size"": ""10""
                    }
                },
                {
                    ""main hall"": {
                        ""description"": ""this is the biggest space"",
                        ""size"": ""200""
                    }
                }
            ]" },
            new Venue { VenueId = 42, VenueDetails = @"{
                ""id"": ""central123"",
                ""name"": ""Central Park"",
                ""description"": ""🏞∮πρότερονแผ่นดินฮั่นเสื่อมሰማይᚻᛖ"",
                ""location"": {
                    ""address"": ""59th St to 110th St"",
                    ""crossStreet"": ""5th Ave to Central Park West"",
                    ""lat"": ""40.78408342593807"",
                    ""lng"": ""-73.96485328674316"",
                    ""postalCode"": ""10028"",
                    ""cc"": ""US"",
                    ""city"": ""New York"",
                    ""state"": ""NY"",
                    ""country"": ""United States"",
                    ""formattedAddress"": [
                        ""59th St to 110th St (5th Ave to Central Park West)"",
                        ""New York, NY 10028"",
                        ""United States""
                    ]
                },
                ""hours"": {
                    ""status"": ""Likely open"",
                    ""isOpen"": ""true"",
                    ""isLocalHoliday"": ""false"",
                    ""timeframes"": [
                        {
                            ""days"": ""Tue–Thu"",
                            ""open"": [{""time"": ""Noon–8:00PM""}]
                        },
                        {
                            ""days"": ""Fri"",
                            ""open"": [{""time"": ""11:00 AM–7:00 PM""}]
                        },
                        {
                            ""days"": ""Sat"",
                            ""open"": [{""time"": ""8:00 AM–8:00PM""}]
                        },
                        {
                            ""days"": ""Sun"",
                            ""open"": [{""time"": ""8:00 AM–7:00 PM""}]
                        }
                    ]
                }
            }" },
        };
        // Create connection to Cloud Spanner.
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
