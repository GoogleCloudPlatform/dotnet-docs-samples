// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START retail_purge_user_event]
// Import user events into a catalog from inline source using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

/// <summary>
/// The purge user event sample class.
/// </summary>
public class PurgeUserEventSample
{
    private const string VisitorId = "test_visitor_id";

    /// <summary>
    /// The the user event.
    /// </summary>
    /// <param name="visitorId">The actual visitor id.</param>
    /// <returns>The usert event.</returns>defaultCatalog
    private static UserEvent GetUserEvent(string visitorId)
    {
        ProductDetail productDetail = new ProductDetail
        {
            Product = new Product
            {
                Id = "test_id"
            },
            Quantity = 3
        };

        UserEvent userEvent = new UserEvent
        {
            EventType = "detail-page-view",
            VisitorId = visitorId,
            EventTime = new Timestamp
            {
                Seconds = DateTime.Now.ToUniversalTime().ToTimestamp().Seconds
            }
        };

        userEvent.ProductDetails.Add(productDetail);

        Console.WriteLine("User Event:");
        Console.WriteLine($"Product detail: {userEvent.ProductDetails}");
        Console.WriteLine($"Event type: {userEvent.EventType}");
        Console.WriteLine($"Visitor id: {userEvent.VisitorId}");
        Console.WriteLine($"Event time: {userEvent.EventTime}");
        Console.WriteLine();

        return userEvent;
    }

    /// <summary>
    /// Get the purge user event request.
    /// </summary>
    /// <param name="defaultCatalog">The default catalog.</param>
    /// <returns>The purge user event request.</returns>
    private static PurgeUserEventsRequest GetPurgeUserEventsRequest(string defaultCatalog)
    {
        PurgeUserEventsRequest purgeRequest = new PurgeUserEventsRequest
        {
            Parent = defaultCatalog,
            Filter = $"visitorId=\"{VisitorId}\"", // To check error handling set invalid filter here.
            Force = true
        };

        Console.WriteLine("Purge user event. request:");
        Console.WriteLine($"Parent: {purgeRequest.Parent}");
        Console.WriteLine($"Filter: {purgeRequest.Filter}");
        Console.WriteLine($"Force: {purgeRequest.Force}");
        Console.WriteLine();

        return purgeRequest;
    }

    /// <summary>
    ///  Call the Retail API to purge user events.
    /// </summary>
    /// <param name="defaultCatalog">The default catalog.</param>
    private static void CallPurgeUserEvents(string defaultCatalog)
    {
        PurgeUserEventsRequest purgeRequest = GetPurgeUserEventsRequest(defaultCatalog);
        UserEventServiceClient client = UserEventServiceClient.Create();
        Operation<PurgeUserEventsResponse, PurgeMetadata> purgeResponse = client.PurgeUserEvents(purgeRequest);

        Console.WriteLine("The purge operation was started:");
        Console.WriteLine(purgeResponse.Name);
        Console.WriteLine();
    }

    /// <summary>
    /// Call the retail API to write the user event.
    /// </summary>
    /// <param name="visitorId">The actual visitor id.</param>
    /// <param name="defaultCatalog">The default catalog.</param>
    private static void WriteUserEvent(string visitorId, string defaultCatalog)
    {
        UserEvent userEventToWrite = GetUserEvent(visitorId);

        WriteUserEventRequest writeUserEventRequest = new WriteUserEventRequest
        {
            Parent = defaultCatalog,
            UserEvent = userEventToWrite
        };

        UserEventServiceClient client = UserEventServiceClient.Create();
        UserEvent userEvent = client.WriteUserEvent(writeUserEventRequest);

        Console.WriteLine("The user event is written:");
        Console.WriteLine(userEvent);
        Console.WriteLine();
    }

    /// <summary>
    /// Perform purge user events.
    /// </summary>
    /// <param name="projectId">The  current project id.</param>
    public void PerformPurgeUserEventOperation(string projectId)
    {
        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog";

        WriteUserEvent(VisitorId, defaultCatalog);
        CallPurgeUserEvents(defaultCatalog);
    }
}
// [END retail_purge_user_event]

/// <summary>
/// The purge user event tutorial class.
/// </summary>
public static class PurgeUserEventTutorial
{
    [Runner.Attributes.Example]
    public static void PerformPurgeUserEventOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new PurgeUserEventSample();
        sample.PerformPurgeUserEventOperation(projectId);
    }
}