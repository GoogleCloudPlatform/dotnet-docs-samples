﻿// Copyright 2021 Google Inc. All Rights Reserved.
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
using Google.LongRunning;
using System;

/// <summary>
/// The import user events gcs sample class.
/// </summary>
public class ImportUserEventsGcsSample
{
    private const string GcsEventsObject = "user_events.json";

    // To check error handling use the json with invalid user event:
    // GcsEventsObject = "user_events_some_invalid.json";

    // Read the bucket name from the environment variable
    private static readonly string BucketName = Environment.GetEnvironmentVariable("EVENTS_BUCKET_NAME");
    private static readonly string GcsBucket = $"gs://{BucketName}";
    private static readonly string GcsErrorsBucket = $"{GcsBucket}/error";

    /// <summary>
    /// Get import user events from gcs request.
    /// </summary>
    /// <param name="gcsObjectName">The name of the Gcs object.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The import user events request.</returns>
    private static ImportUserEventsRequest GetImportUserEventsGcsRequest(string gcsObjectName, string projectId)
    {
        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog"; ;

        // To check error handling paste the invalid catalog name here:
        // defaultCatalog = "invalid_catalog_name";

        var gcsSource = new GcsSource();
        gcsSource.InputUris.Add($"{GcsBucket}/{gcsObjectName}");

        Console.WriteLine("GCS source:");
        Console.WriteLine(gcsSource.InputUris);
        Console.WriteLine();

        var importRequest = new ImportUserEventsRequest
        {
            Parent = defaultCatalog,
            InputConfig = new UserEventInputConfig
            {
                GcsSource = gcsSource
            },
            ErrorsConfig = new ImportErrorsConfig
            {
                GcsPrefix = GcsErrorsBucket
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
    public void ImportProductsFromGcs(string projectId)
    {
        var importGcsRequest = GetImportUserEventsGcsRequest(GcsEventsObject, projectId);
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
    }
}
// [END retail_import_user_events_from_gcs]

/// <summary>
/// The import user events gcs tutorial class.
/// </summary>
public static class ImportUserEventsGcsTutorial
{
    [Runner.Attributes.Example]
    public static void ImportProductsFromGcs()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new ImportUserEventsGcsSample();
        sample.ImportProductsFromGcs(projectId);
    }
}