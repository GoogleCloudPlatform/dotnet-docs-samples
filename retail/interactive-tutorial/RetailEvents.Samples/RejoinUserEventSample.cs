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
    private const string VisitorId = "test_visitor_id";

    /// <summary>
    /// Get the user event.
    /// </summary>
    /// <param name="visitorId">The actual visitor id.</param>
    /// <returns>User event object.</returns>
    private static UserEvent GetUserEvent(string visitorId)
    {
        var productDetail = new ProductDetail
        {
            Product = new Product
            {
                Id = "test_id"
            },
            Quantity = 3
        };

        var userEvent = new UserEvent
        {
            EventType = "detail-page-view",
            VisitorId = visitorId,
            EventTime = new Timestamp
            {
                Seconds = DateTime.Now.ToUniversalTime().ToTimestamp().Seconds
            }
        };

        userEvent.ProductDetails.Add(productDetail);

        Console.WriteLine($"User Event:");
        Console.WriteLine(userEvent);
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
        var rejoinUserEventRequest = new RejoinUserEventsRequest
        {
            Parent = defaultCatalog,
            UserEventRejoinScope = RejoinUserEventsRequest.Types.UserEventRejoinScope.UnjoinedEvents
        };

        Console.WriteLine($"Rejoin user events. request:");
        Console.WriteLine(rejoinUserEventRequest);
        Console.WriteLine();

        return rejoinUserEventRequest;
    }

    /// <summary>
    /// Call the retail API to write the user event.
    /// </summary>
    /// <param name="visitorId">The actual visitor id.</param>
    /// <param name="defaultCatalog">The default catalog.</param>
    private static void WriteUserEvent(string visitorId, string defaultCatalog)
    {
        var userEventToWrite = GetUserEvent(visitorId);

        var writeUserEventRequest = new WriteUserEventRequest
        {
            Parent = defaultCatalog,
            UserEvent = userEventToWrite
        };

        UserEventServiceClient client = UserEventServiceClient.Create();
        UserEvent userEvent = client.WriteUserEvent(writeUserEventRequest);

        Console.WriteLine($"The user event is written:");
        Console.WriteLine(userEvent);
        Console.WriteLine();
    }

    /// <summary>
    /// Call the retail API to purge user event.
    /// </summary>
    /// <param name="visitorId">The actual visitor id.</param>
    /// <param name="defaultCatalog">The default catalog.</param>
    private static void PurgeUserEvent(string visitorId, string defaultCatalog)
    {
        var purgeUserEventRequest = new PurgeUserEventsRequest
        {
            Parent = defaultCatalog,
            Filter = $"visitorId=\"{visitorId}\"",
            Force = true
        };

        UserEventServiceClient client = UserEventServiceClient.Create();
        Operation<PurgeUserEventsResponse, PurgeMetadata> purgeResponse = client.PurgeUserEvents(purgeUserEventRequest);

        Console.WriteLine("The purge operation was started:");
        Console.WriteLine(purgeResponse.Name);
        Console.WriteLine();
    }

    /// <summary>
    /// Call the Retail API to rejoin user event.
    /// </summary>
    /// <param name="defaultCatalog">The default catalog.</param>
    private static void CallRejoinUserEvents(string defaultCatalog)
    {
        RejoinUserEventsRequest rejoinRequest = GetRejoinUserEventRequest(defaultCatalog);
        UserEventServiceClient client = UserEventServiceClient.Create();
        Operation<RejoinUserEventsResponse, RejoinUserEventsMetadata> rejoinResponse = client.RejoinUserEvents(rejoinRequest);

        Console.WriteLine("The rejoin operation was started:");
        Console.WriteLine(rejoinResponse.Name);
        Console.WriteLine();
    }

    /// <summary>
    /// Perform the rejoin user events operation.
    /// </summary>
    /// <param name="projectId">The  current project id.</param>
    public void PerformRejoinUserEventsOperation(string projectId)
    {
        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog";

        // To check the error handling try to pass invalid catalog:
        // defaultCatalog = $"projects/{projectId}/locations/global/catalogs/invalid_catalog";

        WriteUserEvent(VisitorId, defaultCatalog);
        CallRejoinUserEvents(defaultCatalog);
        PurgeUserEvent(VisitorId, defaultCatalog);
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
        var sample = new RejoinUserEventSample();
        sample.PerformRejoinUserEventsOperation(projectId);
    }
}