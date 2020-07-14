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

// [START spanner_query_with_array_parameter]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryWithArrayAsyncSample
{
    public async Task QueryWithArrayAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        // Create a list array of dates to use for querying.
        var exampleArray = new List<DateTime>();
        exampleArray.InsertRange(0, new DateTime[] { DateTime.Parse("2020-10-01"), DateTime.Parse("2020-11-01") });

        // Create connection to Cloud Spanner.
        using (var connection = new SpannerConnection(connectionString))
        {
            var cmd = connection.CreateSelectCommand(
                "SELECT VenueId, VenueName, AvailableDate FROM Venues v, "
                + "UNNEST(v.AvailableDates) as AvailableDate "
                + "WHERE AvailableDate in UNNEST(@ExampleArray)");
            cmd.Parameters.Add("ExampleArray", SpannerDbType.ArrayOf(SpannerDbType.Date), exampleArray);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine(reader.GetFieldValue<string>("VenueId")
                        + " " + reader.GetFieldValue<string>("VenueName")
                        + " " + reader.GetFieldValue<string>("AvailableDate"));
                }
            }
        }
    }
}
// [END spanner_query_with_array_parameter]
