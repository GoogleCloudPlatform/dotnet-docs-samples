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

// [START spanner_insert_datatypes_data]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InsertDatatypesDataAsyncSample
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public byte[] VenueInfo { get; set; }
        public int Capacity { get; set; }
        public List<DateTime> AvailableDates { get; set; }
        public DateTime LastContactDate { get; set; }
        public bool OutdoorVenue { get; set; }
        public float PopularityScore { get; set; }
    }

    public async Task InsertDatatypesDataAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/" +
            $"databases/{databaseId}";

        byte[] exampleBytes1 = Encoding.UTF8.GetBytes("Hello World 1");
        byte[] exampleBytes2 = Encoding.UTF8.GetBytes("Hello World 2");
        byte[] exampleBytes3 = Encoding.UTF8.GetBytes("Hello World 3");

        var availableDates1 = new List<DateTime>() {
            new DateTime(2020, 12, 01, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 12, 02, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 12, 03, 0, 0, 0, DateTimeKind.Utc)
        };

        var availableDates2 = new List<DateTime>() {
            new DateTime(2020, 11, 01, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 11, 05, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 11, 15, 0, 0, 0, DateTimeKind.Utc)
        };

        var availableDates3 = new List<DateTime>() {
            new DateTime(2020, 10, 01, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2020, 10, 07, 0, 0, 0, DateTimeKind.Utc)
        };

        List<Venue> venues = new List<Venue>
        {
                new Venue {VenueId = 4, VenueName = "Venue 4", VenueInfo = exampleBytes1,
                    Capacity = 1800, AvailableDates = availableDates1,
                    LastContactDate = new DateTime(2018,09,02),
                    OutdoorVenue = false, PopularityScore = 0.85543f},
                new Venue {VenueId = 19, VenueName = "Venue 19", VenueInfo = exampleBytes2,
                    Capacity = 6300, AvailableDates = availableDates2,
                    LastContactDate = new DateTime(2019,01,15),
                    OutdoorVenue = true, PopularityScore = 0.98716f},
                new Venue {VenueId = 42, VenueName = "Venue 42", VenueInfo = exampleBytes3,
                    Capacity = 3000, AvailableDates = availableDates3,
                    LastContactDate = new DateTime(2018,10,01),
                    OutdoorVenue = false, PopularityScore = 0.72598f},
        };
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            await connection.OpenAsync();

            await Task.WhenAll(venues.Select(venue =>
            {
                // Insert rows into the Venues table.
                var cmd = connection.CreateInsertCommand("Venues", new SpannerParameterCollection {
                    {"VenueId", SpannerDbType.Int64},
                    {"VenueName", SpannerDbType.String},
                    {"VenueInfo", SpannerDbType.Bytes},
                    {"Capacity", SpannerDbType.Int64},
                    {"AvailableDates", SpannerDbType.ArrayOf(SpannerDbType.Date)},
                    {"LastContactDate", SpannerDbType.Date},
                    {"OutdoorVenue", SpannerDbType.Bool},
                    {"PopularityScore", SpannerDbType.Float64},
                    {"LastUpdateTime", SpannerDbType.Timestamp},
                });

                cmd.Parameters["VenueId"].Value = venue.VenueId;
                cmd.Parameters["VenueName"].Value = venue.VenueName;
                cmd.Parameters["VenueInfo"].Value = venue.VenueInfo;
                cmd.Parameters["Capacity"].Value = venue.Capacity;
                cmd.Parameters["AvailableDates"].Value = venue.AvailableDates;
                cmd.Parameters["LastContactDate"].Value = venue.LastContactDate;
                cmd.Parameters["OutdoorVenue"].Value = venue.OutdoorVenue;
                cmd.Parameters["PopularityScore"].Value = venue.PopularityScore;
                cmd.Parameters["LastUpdateTime"].Value = SpannerParameter.CommitTimestamp;
                return cmd.ExecuteNonQueryAsync();
            }));
            Console.WriteLine("Inserted data.");
        }
    }
}
// [END spanner_insert_datatypes_data]
