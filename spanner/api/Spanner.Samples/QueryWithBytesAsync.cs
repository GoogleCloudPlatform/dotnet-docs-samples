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

// [START spanner_query_with_bytes_parameter]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class QueryWithBytesAsyncSample
{
    public async Task<List<Venue>> QueryWithBytesAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Create a Bytes array to use for querying.
        string sampleText = "Hello World 1";
        byte[] exampleBytes = Encoding.UTF8.GetBytes(sampleText);
        var venues = new List<Venue>();
        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            var cmd = connection.CreateSelectCommand("SELECT VenueId, VenueName FROM Venues WHERE VenueInfo = @ExampleBytes");
            cmd.Parameters.Add("ExampleBytes", SpannerDbType.Bytes, exampleBytes);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var venueId = reader.GetFieldValue<int>("VenueId");
                    var venueName = reader.GetFieldValue<string>("VenueName");
                    Console.WriteLine($"VenueId: {venueId} VenueName: {venueName}");
                    venues.Add(new Venue
                    {
                        VenueId = venueId,
                        VenueName = venueName
                    });
                }
            }
        }
        return venues;
    }
}
// [END spanner_query_with_bytes_parameter]
