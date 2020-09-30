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

// [START spanner_update_data_with_numeric_column]

using Google.Cloud.Spanner.Data;
using Google.Cloud.Spanner.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UpdateDataWithNumericAsyncSample
{
    public class Venue
    {
        public int VenueId { get; set; }
        public SpannerNumeric Revenue { get; set; }
    }

    public async Task UpdateDataWithNumericAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        List<Venue> venues = new List<Venue>
        {
            new Venue { VenueId = 4, Revenue = SpannerNumeric.Parse("35000") },
            new Venue { VenueId = 19, Revenue = SpannerNumeric.Parse("104500") },
            new Venue { VenueId = 42, Revenue = SpannerNumeric.Parse("99999999999999999999999999999.99") },
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
                    { "Revenue", SpannerDbType.Numeric, venue.Revenue }
            });
            return cmd.ExecuteNonQueryAsync();
        }));

        Console.WriteLine("Data updated.");
    }
}
// [END spanner_update_data_with_numeric_column]
