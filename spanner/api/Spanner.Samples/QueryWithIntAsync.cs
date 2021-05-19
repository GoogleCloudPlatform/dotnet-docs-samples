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

// [START spanner_query_with_int_parameter]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryWithIntAsyncSample
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public int Capacity { get; set; }
    }

    public async Task<List<Venue>> QueryWithIntAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Initialize a long variable to use for querying.
        long exampleValue = 3000;

        using var connection = new SpannerConnection(connectionString);
        var cmd = connection.CreateSelectCommand("SELECT VenueId, VenueName, Capacity FROM Venues WHERE Capacity >= @ExampleValue");
        cmd.Parameters.Add("ExampleValue", SpannerDbType.Int64, exampleValue);

        var venues = new List<Venue>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            venues.Add(new Venue
            {
                VenueId = reader.GetFieldValue<int>("VenueId"),
                VenueName = reader.GetFieldValue<string>("VenueName"),
                Capacity = reader.GetFieldValue<int>("Capacity")
            });
        }
        return venues;
    }
}
// [END spanner_query_with_int_parameter]
