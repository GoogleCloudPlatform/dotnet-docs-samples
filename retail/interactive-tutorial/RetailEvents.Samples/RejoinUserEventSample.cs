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

// [START retail_rejoin_user_event]
// Import user events into a catalog from inline source using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

/// <summary>
/// The rejoin user event sample class.
/// </summary>
public class RejoinUserEventSample
{
    /// <summary>
    /// Get the user event.
    /// </summary>
    /// <returns>User event object.</returns>
    public static UserEvent GetUserEvent()
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
            VisitorId = "test_visitor_id",
            EventTime = new Timestamp
            {
                Seconds = DateTime.Now.ToUniversalTime().ToTimestamp().Seconds
            }
        };

        userEvent.ProductDetails.Add(productDetail);

        Console.WriteLine($"User Event:");
        Console.WriteLine($"Product detail: {userEvent.ProductDetails}");
        Console.WriteLine($"Event type: {userEvent.EventType}");
        Console.WriteLine($"Visitor id: {userEvent.VisitorId}");
        Console.WriteLine($"Event time: {userEvent.EventTime}");
        Console.WriteLine();

        return userEvent;
    }

    /// <summary>
    /// Get rejoin user event request.
    /// </summary>
    /// <param name="defaultCatalog">The default catalog.</param>
    /// <returns>The rejoin user events request.</returns>
    private static RejoinUserEventsRequest GetRejoinUserEventRequest(string defaultCatalog)
    {
        RejoinUserEventsRequest rejoinUserEventRequest = new RejoinUserEventsRequest
        {
            Parent = defaultCatalog,
            UserEventRejoinScope = RejoinUserEventsRequest.Types.UserEventRejoinScope.UnjoinedEvents
        };

        Console.WriteLine($"Rejoin user events. request:");
        Console.WriteLine($"Parent: {rejoinUserEventRequest.Parent}");
        Console.WriteLine($"Rejoin scope: {rejoinUserEventRequest.UserEventRejoinScope}");
        Console.WriteLine();

        return rejoinUserEventRequest;
    }

    /// <summary>
    /// Call the Retail API to rejoin user event.
    /// </summary>
    /// <param name="defaultCatalog">The default catalog.</param>
    /// <returns>The completed rejoin response.</returns>
    public static Operation<RejoinUserEventsResponse, RejoinUserEventsMetadata> CallRejoinUserEvents(string defaultCatalog)
    {
        RejoinUserEventsRequest rejoinRequest = GetRejoinUserEventRequest(defaultCatalog);
        UserEventServiceClient client = UserEventServiceClient.Create();
        Operation<RejoinUserEventsResponse, RejoinUserEventsMetadata> rejoinResponse = client.RejoinUserEvents(rejoinRequest);

        Console.WriteLine("The rejoin operation was started:");
        Console.WriteLine(rejoinResponse.Name);
        Console.WriteLine();

        return rejoinResponse;
    }
}
// [END retail_rejoin_user_event]

/// <summary>
/// The rejoin user event tutorial class.
/// </summary>
public static class RejoinUserEventTutorial
{
    [Runner.Attributes.Example]
    public static void PerformRejoinUserEventsOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog";

        // To check the error handling try to pass invalid catalog:
        // defaultCatalog = "projects/{projectId}/locations/global/catalogs/invalid_catalog";

        UserEvent userEventToWrite = RejoinUserEventSample.GetUserEvent();

        WriteUserEventSample.CallWriteUserEvent(defaultCatalog, userEventToWrite);

        RejoinUserEventSample.CallRejoinUserEvents(defaultCatalog);

        PurgeUserEventSample.CallPurgeUserEvents(defaultCatalog);
    }
}