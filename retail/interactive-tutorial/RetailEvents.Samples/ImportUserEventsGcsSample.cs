// Copyright 2022 Google Inc.

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

// Import user events into a catalog from GCS using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using System;

/// <summary>
/// The import user events GCS sample class.
/// </summary>
public class ImportUserEventsGcsSample
{
    /// <summary>
    /// Get import user events from GCS request.
    /// </summary>
    /// <param name="gcsObjectName">The name of the GCS object that contains the list of user events in JSON format.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The import user events request.</returns>
    private static ImportUserEventsRequest GetImportUserEventsGcsRequest(string gcsObjectName, string projectId, string eventsBucketName)
    {
        string gcsBucket = $"gs://{eventsBucketName}";
        string gcsErrorsPrefix = $"{gcsBucket}/error";

        string locationId = "global";
        string catalogId = "default_catalog";
        // To check error handling paste the invalid catalog name here:
        // catalogId = "invalid_catalog_name";
        CatalogName defaultCatalog = new CatalogName(projectId, locationId, catalogId);

        ImportUserEventsRequest importRequest = new ImportUserEventsRequest
        {
            ParentAsCatalogName = defaultCatalog,
            InputConfig = new UserEventInputConfig
            {
                GcsSource = new GcsSource
                {
                    InputUris = { $"{gcsBucket}/{gcsObjectName}" }
                }
            },
            ErrorsConfig = new ImportErrorsConfig
            {
                GcsPrefix = gcsErrorsPrefix
            }
        };

        Console.WriteLine("GCS source:");
        Console.WriteLine(importRequest.InputConfig.GcsSource.InputUris);
        Console.WriteLine();

        Console.WriteLine("Import user events from google cloud source request:");
        Console.WriteLine(importRequest);
        Console.WriteLine();

        return importRequest;
    }

    /// <summary>
    /// Call the Retail API to import user events.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    public static Operation<ImportUserEventsResponse, ImportMetadata> ImportUserEventsFromGcs(string projectId, string eventsBucketName)
    {
        string gcsEventsObject = "user_events.json";
        // To check error handling use the json with invalid user event:
        // gcsEventsObject = "user_events_some_invalid.json";

        ImportUserEventsRequest importGcsRequest = GetImportUserEventsGcsRequest(gcsEventsObject, projectId, eventsBucketName);
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

/// <summary>
/// The import user events GCS tutorial class.
/// </summary>
public static class ImportUserEventsGcsTutorial
{
    [Runner.Attributes.Example]
    public static Operation<ImportUserEventsResponse, ImportMetadata> ImportUserEventsFromGcs()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        string eventsBucketName = Environment.GetEnvironmentVariable("RETAIL_EVENTS_BUCKET_NAME");

        Operation<ImportUserEventsResponse, ImportMetadata> result = ImportUserEventsGcsSample.ImportUserEventsFromGcs(projectId, eventsBucketName);

        return result;
    }
}
