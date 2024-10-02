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

// Purge user events using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using System;

/// <summary>
/// The purge user event sample class.
/// </summary>
public class PurgeUserEventSample
{
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
            Filter = $"visitorId=\"test_visitor_id\"", // To check error handling set invalid filter here.
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
    /// Call the Retail API to purge user events.
    /// </summary>
    /// <param name="defaultCatalog">The default catalog.</param>
    /// <returns>The completed purge response.</returns>
    public static Operation<PurgeUserEventsResponse, PurgeMetadata> CallPurgeUserEvents(string defaultCatalog)
    {
        PurgeUserEventsRequest purgeRequest = GetPurgeUserEventsRequest(defaultCatalog);
        UserEventServiceClient client = UserEventServiceClient.Create();
        Operation<PurgeUserEventsResponse, PurgeMetadata> purgeResponse = client.PurgeUserEvents(purgeRequest);

        Console.WriteLine("The purge operation was started.");

        // The purge operation takes several hours or even days to complete.
        // You may get the name of the operation
        string operationName = purgeResponse.Name;

        // This name can be stored, then the long-running operation retrieved later by name
        Operation<PurgeUserEventsResponse, PurgeMetadata> retrievedResponse = client.PollOncePurgeUserEvents(operationName);

        // Check if the retrieved long-running operation has completed
        if (retrievedResponse.IsCompleted)
        {
            // If it has completed, then access the result
            PurgeUserEventsResponse retrievedResult = retrievedResponse.Result;

            Console.WriteLine("Purged user events count:");
            Console.WriteLine(retrievedResult.PurgedEventsCount);
        }

        return retrievedResponse;
    }
}

/// <summary>
/// The purge user event tutorial class.
/// </summary>
public static class PurgeUserEventTutorial
{
    [Runner.Attributes.Example]
    public static void PerformPurgeUserEventOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog";

        // To check the error handling try to pass invalid catalog:
        // defaultCatalog = $"projects/{projectId}/locations/global/catalogs/invalid_catalog";

        WriteUserEventSample.CallWriteUserEvent(defaultCatalog);

        PurgeUserEventSample.CallPurgeUserEvents(defaultCatalog);
    }
}
