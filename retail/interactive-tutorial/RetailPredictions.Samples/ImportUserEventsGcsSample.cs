﻿using Google.Cloud.Retail.V2;
using Google.LongRunning;
using System;

namespace RetailPredictions.Samples
{
    public class ImportUserEventsGcsSample
    {
        private const string GcsEventsObject = "user_events.json";

        // To check error handling use the json with invalid user event:
        // private const string GcsEventsObject = "user_events_some_invalid.json";

        /// <summary>
        /// Get import user events from gcs request.
        /// </summary>
        /// <param name="gcsObjectName">The name of the Gcs object.</param>
        /// <param name="projectId">The current project id.</param>
        /// <returns>The import user events request.</returns>
        private static ImportUserEventsRequest GetImportUserEventsGcsRequest(string gcsObjectName, string projectId, string bucketName)
        {
            string eventsBucketName = bucketName ?? Environment.GetEnvironmentVariable("EVENTS_BUCKET_NAME");
            string gcsBucket = $"gs://{eventsBucketName}";
            string gcsErrorsBucket = $"{gcsBucket}/error";

            string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog";

            // To check error handling paste the invalid catalog name here:
            // defaultCatalog = "invalid_catalog_name";

            GcsSource gcsSource = new GcsSource();
            gcsSource.InputUris.Add($"{gcsBucket}/{gcsObjectName}");

            Console.WriteLine("GCS source:");
            Console.WriteLine(gcsSource.InputUris);
            Console.WriteLine();

            ImportUserEventsRequest importRequest = new ImportUserEventsRequest
            {
                Parent = defaultCatalog,
                InputConfig = new UserEventInputConfig
                {
                    GcsSource = gcsSource
                },
                ErrorsConfig = new ImportErrorsConfig
                {
                    GcsPrefix = gcsErrorsBucket
                }
            };

            Console.WriteLine("Import user events from google cloud source. request:");
            Console.WriteLine(importRequest);
            Console.WriteLine();

            return importRequest;
        }

        /// <summary>
        /// Call the Retail API to import user events.
        /// </summary>
        /// <param name="projectId">The current project id.</param>
        public Operation<ImportUserEventsResponse, ImportMetadata> ImportUserEventsFromGcs(string projectId, string bucketName = null)
        {
            ImportUserEventsRequest importGcsRequest = GetImportUserEventsGcsRequest(GcsEventsObject, projectId, bucketName);
            UserEventServiceClient client = UserEventServiceClient.Create();
            Operation<ImportUserEventsResponse, ImportMetadata> importResponse = client.ImportUserEvents(importGcsRequest);

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
    // [END retail_import_user_events_from_gcs]

    /// <summary>
    /// The import user events gcs tutorial class.
    /// </summary>
    public static class ImportUserEventsGcsTutorial
    {
        [Runner.Attributes.Example]
        public static Operation<ImportUserEventsResponse, ImportMetadata> ImportUserEventsFromGcs()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            var sample = new ImportUserEventsGcsSample();

            string createdBucketName = EventsCreateGcsBucket.PerformCreationOfEventsGcsBucket();

            Operation<ImportUserEventsResponse, ImportMetadata> importResponse = sample.ImportUserEventsFromGcs(projectId, createdBucketName);

            return importResponse;
        }
    }
}
