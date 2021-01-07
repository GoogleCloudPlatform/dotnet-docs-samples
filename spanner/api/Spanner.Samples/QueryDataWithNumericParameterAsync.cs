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

// [START spanner_query_with_numeric_parameter]

using Google.Cloud.Spanner.Data;
using Google.Cloud.Spanner.V1;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryDataWithNumericParameterAsyncSample
{
    public class Venue
    {
        public int VenueId { get; set; }
        public SpannerNumeric Revenue { get; set; }
    }

    public async Task<List<Venue>> QueryDataWithNumericParameterAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        using var connection = new SpannerConnection(connectionString);
        using var cmd = connection.CreateSelectCommand(
            "SELECT VenueId, Revenue FROM Venues WHERE Revenue < @maxRevenue",
            new SpannerParameterCollection
            {
                { "maxRevenue", SpannerDbType.Numeric, SpannerNumeric.Parse("100000") }
            });

        var venues = new List<Venue>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            venues.Add(new Venue
            {
                VenueId = reader.GetFieldValue<int>("VenueId"),
                Revenue = reader.GetFieldValue<SpannerNumeric>("Revenue")
            });
        }
        return venues;
    }
}
// [END spanner_query_with_numeric_parameter]
