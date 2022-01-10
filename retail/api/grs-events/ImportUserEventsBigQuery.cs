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

// [START retail_import_user_events_from_big_query]
// Import user events into a catalog from BigQuery using Retail API

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace grs_events
{
    public static class ImportUserEventsBigQuery
    {
        private const string Endpoint = "retail.googleapis.com";
        private const string DataSetId = "user_events";
        private const string DataSchema = "user_event";
        private const string TableId = "events";

        // TO CHECK ERROR HANDLING USE THE TABLE OF INVALID USER EVENTS:
        // TableId = "events_some_invalid"
        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string ProjectId = Environment.GetEnvironmentVariable("PROJECT_ID");

        private static readonly string DefaultCatalog = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog";

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

        // Get import user events big query request
        private static ImportUserEventsRequest GetImportUserEventsBigQueryRequest()
        {
            // TO CHECK ERROR HANDLING PASTE THE INVALID CATALOG NAME HERE:
            // DefaultCatalog = "invalid_catalog_name"
            var bigQuerySource = new BigQuerySource
            {
                ProjectId = ProjectId,
                DatasetId = DataSetId,
                TableId = TableId,
                DataSchema = DataSchema
            };

            var inputConfig = new UserEventInputConfig
            {
                BigQuerySource = bigQuerySource
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

            Console.WriteLine("\nImport user events from BigQuery source. request: \n\n" + importRequestJson);
            return importRequest;
        }

        // Call the Retail API to import user events
        [Attributes.Example]
        public static void ImportProductsFromBigQuery()
        {
            var importBigQueryRequest = GetImportUserEventsBigQueryRequest();
            var importResponse = GetUserEventsServiceClient().ImportUserEvents(importBigQueryRequest);

            Console.WriteLine("\nThe operation was started: \n" + importResponse.Name);
            Console.WriteLine("\nPlease wait till opeartion is done\n");

            var importResult = importResponse.PollUntilCompleted();

            Console.WriteLine("Import user events operation is done");
            Console.WriteLine("Number of successfully imported events: " + importResult.Metadata.SuccessCount);
            Console.WriteLine("Number of failures during the importing: " + importResult.Metadata.FailureCount);
            Console.WriteLine("\nOperation result: \n" + importResult.Result);
        }
    }
}
// [END retail_import_user_events_from_big_query]