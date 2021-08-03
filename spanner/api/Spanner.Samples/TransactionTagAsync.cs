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

// [START spanner_set_transaction_tag]

using Google.Cloud.Spanner.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TransactionTagAsyncSample
{
    public class Venue
    {
        public long VenueId { get; set; }
        public string VenueName { get; set; }
        public long Capacity { get; set; }
    }
    
    public async Task<List<Venue>> TransactionTagAsync(string projectId, string instanceId, string databaseId)
    {
        string connectionString = $"Data Source=projects/{projectId}/instances/{instanceId}/databases/{databaseId}";
        using var connection = new SpannerConnection(connectionString);
        await connection.OpenAsync();

        return await connection.RunWithRetriableTransactionAsync(async transaction =>
        {
            var venues = new List<Venue>();
            // Sets the transaction tag to "app=concert,env=dev".
            // This transaction tag will be applied to all the individual operations inside
            // the transaction.
            transaction.Tag = "app=concert,env=dev";
            var selectCommand = connection.CreateSelectCommand(
                @"SELECT VenueId, VenueName, Capacity
                  FROM Venues
                  WHERE OutdoorVenue = @outdoorVenue",
                new SpannerParameterCollection
                {
                    new SpannerParameter("outdoorVenue", SpannerDbType.Bool, false)
                });
            selectCommand.Transaction = transaction;
            // Sets the request tag to "app=concert,env=dev,action=select".
            // This request tag will only be set on this request.
            selectCommand.Tag = "app=concert,env=dev,action=select";
            using var reader = await selectCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var venue = new Venue
                {
                    VenueId = reader.GetFieldValue<long>("VenueId"),
                    VenueName = reader.GetFieldValue<string>("VenueName"),
                    Capacity = reader.GetFieldValue<long>("Capacity")
                };
                venues.Add(venue);
                // Update the capacity of the venue.
                venue.Capacity /= 4;
                var updateCommand = connection.CreateDmlCommand(
                    "UPDATE Venues SET Capacity = @capacity WHERE VenueId = @venueId",
                    new SpannerParameterCollection
                    {
                        {"capacity", SpannerDbType.Int64, venue.Capacity},
                        {"venueId", SpannerDbType.Int64, venue.VenueId}
                    });
                updateCommand.Transaction = transaction;
                // Sets the request tag to "app=concert,env=dev,action=update".
                // This request tag will only be set on this request.
                updateCommand.Tag = "app=concert,env=dev,action=update";
                await updateCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"Capacity of {venue.VenueName} updated to {venue.Capacity}");
            }
            return venues;
        });
    }
}
// [END spanner_set_transaction_tag]
