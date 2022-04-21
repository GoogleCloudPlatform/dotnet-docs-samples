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

// [START retail_import_user_events_from_inline_source]
// Import user events into a catalog from inline source using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

/// <summary>
/// The import user events inline source sample class.
/// </summary>
public class ImportUserEventsInlineSourceSample
{
    /// <summary>
    /// Get user events for import.
    /// </summary>
    /// <returns>List of user events.</returns>
    private static List<UserEvent> GetUserEvents()
    {
        const short userEventsCount = 3;
        List<UserEvent> userEvents = new List<UserEvent>();

        for (int i = 1; i <= userEventsCount; i++)
        {
            UserEvent userEvent = new UserEvent
            {
                EventType = "home-page-view", // EventType = "invalid",
                VisitorId = "test_visitor_id",
                EventTime = new Timestamp
                {
                    Seconds = DateTime.Now.ToUniversalTime().ToTimestamp().Seconds
                }
            };

            userEvents.Add(userEvent);

            Console.WriteLine($"User Event {i}:");
            Console.WriteLine(userEvent);
            Console.WriteLine();
        }

        return userEvents;
    }

    /// <summary>
    ///  Get import user events from inline source request.
    /// </summary>
    /// <param name="userEventToImport">List of user events to import.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The import user events request.</returns>
    private static ImportUserEventsRequest GetImportUserEventsInlineSourceRequest(List<UserEvent> userEventToImport, string projectId)
    {
        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog";

        // To check error handling paste the invalid catalog name here:
        // defaultCatalog = "invalid_catalog_name";

        UserEventInlineSource inlineSource = new UserEventInlineSource();
        inlineSource.UserEvents.AddRange(userEventToImport);

        ImportUserEventsRequest importRequest = new ImportUserEventsRequest
        {
            Parent = defaultCatalog,
            InputConfig = new UserEventInputConfig
            {
                UserEventInlineSource = inlineSource
            }
        };

        Console.WriteLine("Import user events from inline source. request:");
        Console.WriteLine(importRequest);
        Console.WriteLine();

        return importRequest;
    }

    /// <summary>
    /// Call the Retail API to import user events.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    public Operation<ImportUserEventsResponse, ImportMetadata> ImportUserEventsFromInlineSource(string projectId)
    {
        List<UserEvent> userEvents = GetUserEvents();
        ImportUserEventsRequest importRequest = GetImportUserEventsInlineSourceRequest(userEvents, projectId);
        UserEventServiceClient client = UserEventServiceClient.Create();
        Operation<ImportUserEventsResponse, ImportMetadata> importResponse = client.ImportUserEvents(importRequest);

        Console.WriteLine("The operation was started:");
        Console.WriteLine(importResponse.Name);
        Console.WriteLine();

        Console.WriteLine("Please wait till opeartion is done");
        Console.WriteLine();

        Operation<ImportUserEventsResponse, ImportMetadata> importResult = importResponse.PollUntilCompleted();

        Console.WriteLine("Import user events operation is done");
        Console.WriteLine();

        Console.WriteLine("Number of successfully imported events: " + importResult.Metadata.SuccessCount);
        Console.WriteLine("Number of failures during the importing: " + importResult.Metadata.FailureCount);
        Console.WriteLine();
        Console.WriteLine("Operation result:");
        Console.WriteLine(importResult.Result);
        Console.WriteLine();

        return importResult;
    }
}
// [END retail_import_user_events_from_inline_source]

/// <summary>
/// The import user events inline source tutorial class.
/// </summary>
public static class ImportUserEventsInlineSourceTutorial
{
    [Runner.Attributes.Example]
    public static Operation<ImportUserEventsResponse, ImportMetadata> ImportUserEventsFromInlineSource()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new ImportUserEventsInlineSourceSample();
        return sample.ImportUserEventsFromInlineSource(projectId);
    }
}