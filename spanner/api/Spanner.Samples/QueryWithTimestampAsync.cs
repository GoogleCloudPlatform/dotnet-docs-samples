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

// [START spanner_query_with_timestamp_parameter]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryWithTimestampAsyncSample
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public DateTime? LastUpdateTime { get; set; }
    }

    public async Task<List<Venue>> QueryWithTimestampAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Initialize a DateTime timestamp variable to use for querying.
        DateTime exampleTimestamp = DateTime.UtcNow;

        using var connection = new SpannerConnection(connectionString);
        var cmd = connection.CreateSelectCommand("SELECT VenueId, VenueName, LastUpdateTime FROM Venues WHERE LastUpdateTime < @ExampleTimestamp");
        cmd.Parameters.Add("ExampleTimestamp", SpannerDbType.Timestamp, exampleTimestamp);

        var venues = new List<Venue>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            venues.Add(new Venue
            {
                VenueId = reader.GetFieldValue<int>("VenueId"),
                VenueName = reader.GetFieldValue<string>("VenueName"),
                LastUpdateTime = reader["LastUpdateTime"] != DBNull.Value ? reader.GetFieldValue<DateTime?>("LastUpdateTime") : null
            });
        }
        return venues;
    }
}
// [END spanner_query_with_timestamp_parameter]
