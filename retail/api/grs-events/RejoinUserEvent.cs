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
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace grs_events
{
    public static class RejoinUserEvent
    {
        private const string Endpoint = "retail.googleapis.com";
        private const string VisitorId = "test_visitor_id";
        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string DefaultCatalog = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog";

        // Get user event
        private static UserEvent GetUserEvent(string visitorId)
        {
            var timeStamp = new Timestamp
            {
                Seconds = DateTime.Now.ToUniversalTime().ToTimestamp().Seconds
            };

            var product = new Product
            {
                Id = "test_id"
            };

            var productDetail = new ProductDetail
            {
                Product = product,
                Quantity = 3
            };

            var userEvent = new UserEvent
            {
                EventType = "detail-page-view",
                VisitorId = visitorId,
                EventTime = timeStamp,
            };

            userEvent.ProductDetails.Add(productDetail);

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var userEventJson = JsonConvert.SerializeObject(userEvent, jsonSerializeSettings);

            Console.WriteLine($"\nUser Event: \n{userEventJson}\n");
            return userEvent;
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

        // Get rejoin user event request
        private static RejoinUserEventsRequest GetRejoinUserEventRequest()
        {
            // TO CHECK THE ERROR HANDLING TRY TO PASS INVALID CATALOG:
            // DefaultCatalog = "projects/{ProjectNumber}/locations/global/catalogs/invalid_catalog"
            var rejoinUserEventRequest = new RejoinUserEventsRequest
            {
                Parent = DefaultCatalog,
                UserEventRejoinScope = RejoinUserEventsRequest.Types.UserEventRejoinScope.UnjoinedEvents
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var rejoinUserEventRequestJson = JsonConvert.SerializeObject(rejoinUserEventRequest, jsonSerializeSettings);

            Console.WriteLine($"\nRejoin user events. request: \n{rejoinUserEventRequestJson}\n");
            return rejoinUserEventRequest;
        }

        // Write user event
        private static void WriteUserEvent(string visitorId)
        {
            var userEventToWrite = GetUserEvent(visitorId);

            var writeUserEventRequest = new WriteUserEventRequest
            {
                Parent = DefaultCatalog,
                UserEvent = userEventToWrite
            };

            var userEvent = GetUserEventsServiceClient().WriteUserEvent(writeUserEventRequest);

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var userEventJson = JsonConvert.SerializeObject(userEvent, jsonSerializeSettings);

            Console.WriteLine($"\nThe user event is written: \n" + userEventJson);
        }

        private static void PurgeUserEvent(string visitorId)
        {
            var purgeUserEventRequest = new PurgeUserEventsRequest
            {
                Parent = DefaultCatalog,
                Filter = $"visitorId=\"{visitorId}\"",
                Force = true
            };

            var purgeResponse = GetUserEventsServiceClient().PurgeUserEvents(purgeUserEventRequest);

            Console.WriteLine("\nThe purge operation was started: \n" + purgeResponse.Name);
        }
        

        // Call the Retail API to rejoin user event
        private static void CallRejoinUserEvents()
        {
            var rejoinRequest = GetRejoinUserEventRequest();
            var rejoinResponse = GetUserEventsServiceClient().RejoinUserEvents(rejoinRequest);

            Console.WriteLine("\nThe rejoin operation was started: \n" + rejoinResponse.Name);
        }

        // Perform rejoin user events
        [Attributes.Example]
        public static void PerformRejoinUserEventsOperation()
        {
            WriteUserEvent(VisitorId);
            CallRejoinUserEvents();
            PurgeUserEvent(VisitorId);
        }
    }
}
// [END retail_rejoin_user_event]