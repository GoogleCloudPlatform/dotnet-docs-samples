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

// [START retail_import_user_events_from_gcs]
// Import user events into a catalog from GCS using Retail API

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace grs_events
{
    public static class ImportUserEventsGcs
    {
        private const string Endpoint = "retail.googleapis.com";
        private const string GcsEventsObject = "user_events.json";

        // TO CHECK ERROR HANDLING USE THE JSON WITH INVALID USER EVENT:
        // gcs_events_object = "user_events_some_invalid.json"

        // Read the project number from the environment variable
        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");

        private static readonly string DefaultCatalog = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog";

        // Read the bucket name from the environment variable
        private static readonly string BucketName = Environment.GetEnvironmentVariable("EVENTS_BUCKET_NAME");
        private static readonly string GcsBucket = $"gs://{BucketName}";
        private static readonly string GcsErrorsBucket = $"{GcsBucket}/error";

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

        // Get import user events from gcs request
        private static ImportUserEventsRequest GetImportUserEventsGcsRequest(string gcsObjectName)
        {
            // TO CHECK ERROR HANDLING PASTE THE INVALID CATALOG NAME HERE:
            // DefaultCatalog = "invalid_catalog_name"
            var gcsSource = new GcsSource();
            gcsSource.InputUris.Add($"{GcsBucket}/{gcsObjectName}");

            var inputConfig = new UserEventInputConfig
            {
                GcsSource = gcsSource
            };

            Console.WriteLine("\nGCS source: \n" + gcsSource.InputUris);

            var errorsConfig = new ImportErrorsConfig
            {
                GcsPrefix = GcsErrorsBucket
            };

            var importRequest = new ImportUserEventsRequest
            {
                Parent = DefaultCatalog,
                InputConfig = inputConfig,
                ErrorsConfig = errorsConfig
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var importRequestJson = JsonConvert.SerializeObject(importRequest, jsonSerializeSettings);

            Console.WriteLine("\nImport user events from google cloud source. request: \n\n" + importRequestJson);
            return importRequest;
        }

        // Call the Retail API to import user events
        [Attributes.Example]
        public static void ImportProductsFromGcs()
        {
            var importGcsRequest = GetImportUserEventsGcsRequest(GcsEventsObject); 
            var importResponse = GetUserEventsServiceClient().ImportUserEvents(importGcsRequest);

            Console.WriteLine("\nThe operation was started: \n" + importResponse.Name);
            Console.WriteLine("\nPlease wait till opeartion is done");

            var importResult = importResponse.PollUntilCompleted();

            Console.WriteLine("Import user events operation is done\n");
            Console.WriteLine("Number of successfully imported events: " + importResult.Metadata.SuccessCount);
            Console.WriteLine("Number of failures during the importing: " + importResult.Metadata.FailureCount);
            Console.WriteLine("\nOperation result: \n" + importResult.Result);
        }
    }
}
// [END retail_import_user_events_from_gcs]