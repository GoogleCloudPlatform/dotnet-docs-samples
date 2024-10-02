// Copyright 2021 Google Inc.

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

// Rejoin user events using Retail API

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
            EventTime = DateTime.UtcNow.ToTimestamp()
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

        Console.WriteLine("The rejoin operation was started.");

        // The rejoin operation takes several hours or even days to complete.
        // You may get the name of the operation
        string operationName = rejoinResponse.Name;

        // This name can be stored, then the long-running operation retrieved later by name
        Operation<RejoinUserEventsResponse, RejoinUserEventsMetadata> retrievedResponse = client.PollOnceRejoinUserEvents(operationName);

        // Check if the retrieved long-running operation has completed
        if (retrievedResponse.IsCompleted)
        {
            // If it has completed, then access the result
            RejoinUserEventsResponse retrievedResult = retrievedResponse.Result;

            Console.WriteLine("Rejoined user events count:");
            Console.WriteLine(retrievedResult.RejoinedUserEventsCount);
        }

        return rejoinResponse;
    }
}

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
        // defaultCatalog = $"projects/{projectId}/locations/global/catalogs/invalid_catalog";

        WriteUserEventSample.CallWriteUserEvent(defaultCatalog);

        RejoinUserEventSample.CallRejoinUserEvents(defaultCatalog);
    }
}
