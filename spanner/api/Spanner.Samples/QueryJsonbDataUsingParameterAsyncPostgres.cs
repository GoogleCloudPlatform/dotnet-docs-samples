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

// [START spanner_postgresql_jsonb_query_parameter]

using Google.Cloud.Spanner.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueryJsonbDataUsingParameterAsyncPostgresSample
{
    public async Task<List<VenueInformation>> QueryJsonbDataUsingParameterAsyncPostgres(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        // Get all the venues with a rating greater than 2.
        /* Details is a column of type JSONB. Some of the data persisted in the Details column has the following structure:
           [{
                 "name": "string",
                 "available": true,
                 "rating": int // This field is optional.
           }] */
        using var command = connection.CreateSelectCommand(
            "SELECT venueid, details FROM VenueInformation WHERE CAST(details ->> 'rating' AS INTEGER) > $1",
            new SpannerParameterCollection
            {
                { "p1", SpannerDbType.Int64, 2 }
            });
        var venues = new List<VenueInformation>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            venues.Add(new VenueInformation
            {
                VenueId = reader.GetFieldValue<int>("venueid"),
                Details = reader.GetFieldValue<string>("details")
            });
        }
        return venues;
    }

    public struct VenueInformation
    {
        public int VenueId { get; set; }
        public string Details { get; set; }
    }
}
// [END spanner_postgresql_jsonb_query_parameter]
