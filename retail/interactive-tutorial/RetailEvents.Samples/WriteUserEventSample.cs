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

// Import user events into a catalog from inline source using Retail API

using Google.Cloud.Retail.V2;
using Google.Protobuf.WellKnownTypes;
using System;

/// <summary>
/// Write user event sample class.
/// </summary>
public class WriteUserEventSample
{
    /// <summary>
    /// Get user event.
    /// </summary>
    /// <returns>User event.</returns>
    private static UserEvent GetUserEvent()
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
    /// Get write user event request
    /// </summary>
    /// <param name="userEventToWrite">The user event object to write.</param>
    /// <param name="defaultCatalog">The default catalog.</param>
    /// <returns>Write user event request.</returns>
    private static WriteUserEventRequest GetWriteUserEventRequest(UserEvent userEventToWrite, string defaultCatalog)
    {
        WriteUserEventRequest writeUserEventRequest = new WriteUserEventRequest
        {
            Parent = defaultCatalog,
            UserEvent = userEventToWrite
        };

        Console.WriteLine($"Write user event. request:");
        Console.WriteLine($"Parent: {writeUserEventRequest.Parent}");
        Console.WriteLine($"User event to write:");
        Console.WriteLine($"User event type: {userEventToWrite.EventType}");
        Console.WriteLine($"User event visitor id: {userEventToWrite.VisitorId}");
        Console.WriteLine($"User event time: {userEventToWrite.EventTime}");
        Console.WriteLine();

        return writeUserEventRequest;
    }

    /// <summary>
    /// Call the Retail API to write user event.
    /// </summary>
    /// <param name="defaultCatalog">The default catalog.</param>
    /// <returns>Written user event.</returns>
    public static UserEvent CallWriteUserEvent(string defaultCatalog)
    {
        UserEvent userEventToWrite = GetUserEvent();
        WriteUserEventRequest writeRequest = GetWriteUserEventRequest(userEventToWrite, defaultCatalog);
        UserEventServiceClient client = UserEventServiceClient.Create();
        UserEvent userEvent = client.WriteUserEvent(writeRequest);

        Console.WriteLine("Written user event:" );
        Console.WriteLine(userEvent);
        Console.WriteLine();

        return userEvent;
    }
}

/// <summary>
/// Write user event tutorial class.
/// </summary>
public static class WriteUserEventTutorial
{
    [Runner.Attributes.Example]
    public static void PerformWriteUserEventsOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog";

        // To check the error handling try to pass invalid catalog:
        // defaultCatalog = $"projects/{projectId}/locations/global/catalogs/invalid_catalog";

        WriteUserEventSample.CallWriteUserEvent(defaultCatalog);

        PurgeUserEventSample.CallPurgeUserEvents(defaultCatalog);
    }
}
