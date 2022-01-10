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
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace grs_events
{
    public static class ImportUserEventsInlineSource
    {
        private const string Endpoint = "retail.googleapis.com";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string DefaultCatalog = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog";

        // TO CHECK ERROR HANDLING PASTE THE INVALID CATALOG NAME HERE:
        // DefaultCatalog = "invalid_catalog_name"

        private static readonly Random random = new();

        public static string RandomAlphanumericString(int length)
        {

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Get user events service client
        private static UserEventServiceClient GetUserEventsServiceClient()
        {
            var userEventServiceClientBuilder = new UserEventServiceClientBuilder
            {
                Endpoint = Endpoint
            };

            var userEventServiceClient = userEventServiceClientBuilder.Build();
            return userEventServiceClient;
        }

        // Get user events for import
        private static List<UserEvent> GetUserEvents()
        {
            var userEvents = new List<UserEvent>();
            const short userEventsCount = 3;

            for (int i = 1; i <= userEventsCount; i++)
            {
                var timeStamp = new Timestamp
                {
                    Seconds = DateTime.Now.ToUniversalTime().ToTimestamp().Seconds
                };

                var userEvent = new UserEvent
                {
                    EventType = "home-page-view",
                    // event_type = "invalid",
                    VisitorId = "test_visitor_id",
                    EventTime = timeStamp,
                };

                userEvents.Add(userEvent);

                var jsonSerializeSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                };

                var userEventJson = JsonConvert.SerializeObject(userEvent, jsonSerializeSettings);

                Console.WriteLine($"\nUser Event {i}: \n{userEventJson}\n");
            }

            return userEvents;
        }

        // Get import user events from inline source request
        private static ImportUserEventsRequest GetImportUserEventsInlineSourceRequest(List<UserEvent> userEventToImport)
        {
            var inlineSource = new UserEventInlineSource();
            inlineSource.UserEvents.AddRange(userEventToImport);

            var inputConfig = new UserEventInputConfig
            {
                UserEventInlineSource = inlineSource
            };

            var importRequest = new ImportUserEventsRequest
            {
                Parent = DefaultCatalog,
                InputConfig = inputConfig
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var importRequestJson = JsonConvert.SerializeObject(importRequest, jsonSerializeSettings);

            Console.WriteLine("\nImport user events from inline source. request: \n\n" + importRequestJson);
            return importRequest;
        }

        // Call the Retail API to import user events
        [Attributes.Example]
        public static void ImportProductsFromInlineSource()
        {
            var userEvents = GetUserEvents();
            var importRequest = GetImportUserEventsInlineSourceRequest(userEvents);
            var importResponse = GetUserEventsServiceClient().ImportUserEvents(importRequest);

            Console.WriteLine("\nThe operation was started: \n" + importResponse.Name);
            Console.WriteLine("Please wait till opeartion is done");

            var importResult = importResponse.PollUntilCompleted();

            Console.WriteLine("Import user events operation is done\n");
            Console.WriteLine("Number of successfully imported events: " + importResult.Metadata.SuccessCount);
            Console.WriteLine("Number of failures during the importing: " + importResult.Metadata.FailureCount);
            Console.WriteLine("\nOperation result: \n" + importResult.Result);
        }
    }
}
// [END retail_import_user_events_from_inline_source]