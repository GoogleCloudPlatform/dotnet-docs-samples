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

// [START spanner_insert_data_with_timestamp_column]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class WriteDataWithTimestampAsyncSample
{
    public class Performance
    {
        public int SingerId { get; set; }
        public int VenueId { get; set; }
        public DateTime EventDate { get; set; }
        public long Revenue { get; set; }
    }

    public async Task<int> WriteDataWithTimestampAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";

        List<Performance> performances = new List<Performance>
        {
            new Performance { SingerId = 1, VenueId = 4, EventDate = DateTime.Parse("2017-10-05"), Revenue = 11000 },
            new Performance { SingerId = 1, VenueId = 19, EventDate = DateTime.Parse("2017-11-02"), Revenue = 15000 },
            new Performance { SingerId = 2, VenueId = 42, EventDate = DateTime.Parse("2017-12-23"), Revenue = 7000 },
        };
        // Create connection to Cloud Spanner.
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        // Insert rows into the Performances table.
        var rowCountAarray = await Task.WhenAll(performances.Select(performance =>
        {
            var cmd = connection.CreateInsertCommand("Performances", new SpannerParameterCollection
            {
                { "SingerId", SpannerDbType.Int64, performance.SingerId },
                { "VenueId", SpannerDbType.Int64, performance.VenueId },
                { "EventDate", SpannerDbType.Date, performance.EventDate },
                { "Revenue", SpannerDbType.Int64, performance.Revenue },
                { "LastUpdateTime", SpannerDbType.Timestamp, SpannerParameter.CommitTimestamp },
            });
            return cmd.ExecuteNonQueryAsync();
        }));
        return rowCountAarray.Sum();
    }
}
// [END spanner_insert_data_with_timestamp_column]
