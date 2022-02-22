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

// [START retail_write_user_event]
// Import user events into a catalog from inline source using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

/// <summary>
/// Write user event sample class.
/// </summary>
public class WriteUserEventSample
{
    private const string VisitorId = "test_visitor_id";

    /// <summary>
    /// Get user event.
    /// </summary>
    /// <returns>User event.</returns>
    private static UserEvent GetUserEvent()
    {
        var timeStamp = new Timestamp
        {
            Seconds = DateTime.Now.ToUniversalTime().ToTimestamp().Seconds
        };

        var userEvent = new UserEvent
        {
            EventType = "home-page-view",
            VisitorId = VisitorId,
            EventTime = timeStamp
        };

        Console.WriteLine($"User Event:");
        Console.WriteLine(userEvent);
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
        var writeUserEventRequest = new WriteUserEventRequest
        {
            Parent = defaultCatalog,
            UserEvent = userEventToWrite
        };

        Console.WriteLine($"Write user event. request:");
        Console.WriteLine(writeUserEventRequest);
        Console.WriteLine();

        return writeUserEventRequest;
    }

    /// <summary>
    /// Call the retail API to purge the user event.
    /// </summary>
    /// <param name="visitorId">The avtual visitor id.</param>
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
    /// Call the Retail API to write user event.
    /// </summary>
    /// <param name="defaultCatalog">The default catalog.</param>
    private static void CallWriteUserEvent(string defaultCatalog)
    {
        UserEvent userEventToWrite = GetUserEvent();
        WriteUserEventRequest writeRequest = GetWriteUserEventRequest(userEventToWrite, defaultCatalog);
        UserEventServiceClient client = UserEventServiceClient.Create();
        UserEvent userEvent = client.WriteUserEvent(writeRequest);

        Console.WriteLine("Written user event:" );
        Console.WriteLine(userEvent);
        Console.WriteLine();
    }

    /// <summary>
    /// Perform write user event operation.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    public void PerformWriteUserEventsOperation(string projectId)
    {
        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog";

        // To check the error handling try to pass invalid catalog:
        // defaultCatalog = "projects/{projectId}/locations/global/catalogs/invalid_catalog";

        CallWriteUserEvent(defaultCatalog);
        PurgeUserEvent(VisitorId, defaultCatalog);
    }
}
// [END retail_write_user_event]

/// <summary>
/// Write user event tutorial class.
/// </summary>
public static class WriteUserEventTutorial
{
    [Runner.Attributes.Example]
    public static void PerformWriteUserEventsOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new WriteUserEventSample();
        sample.PerformWriteUserEventsOperation(projectId);
    }
}