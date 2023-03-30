// Copyright 2022 Google Inc.
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

// [START spanner_postgresql_jsonb_update_data]

using Google.Cloud.Spanner.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UpdateDataWithJsonbAsyncPostgresSample
{
    public async Task UpdateDataWithJsonbAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        List<VenueInformation> venueInformationList = new List<VenueInformation>
        {
            // If you are using .NET Core 3.1 or later, you can use System.Text.Json for serialization instead.
            new VenueInformation
            {
                VenueId = 19,
                Details = JsonConvert.SerializeObject(new
                {
                    rating = 9,
                    open = true,
                })
            },
            new VenueInformation
            {
                VenueId = 4,
                // In the case of repeated field names in the JSON, PostgreSQL JSONB will keep the value of the last field appearance.
                // For instance, in the following example, the value for name will be room 3.
                Details = @"
                {
                    ""name"": ""room 2"",
                    ""available"": false,
                    ""name"": ""room 3""
                }"
            },
            new VenueInformation
            {
                VenueId = 42,
                Details = JsonConvert.SerializeObject(new
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

        await Task.WhenAll(venueInformationList.Select(venue =>
        {
            // Update rows in the Venues table.
            using var cmd = connection.CreateUpdateCommand("VenueInformation", new SpannerParameterCollection
            {
                { "VenueId", SpannerDbType.Int64, venue.VenueId },
                { "Details", SpannerDbType.PgJsonb, venue.Details }
            });
            return cmd.ExecuteNonQueryAsync();
        }));
        Console.WriteLine("Data updated.");
    }

    public struct VenueInformation
    {
        public int VenueId { get; set; }
        public string Details { get; set; }
    }
}
// [END spanner_postgresql_jsonb_update_data]
