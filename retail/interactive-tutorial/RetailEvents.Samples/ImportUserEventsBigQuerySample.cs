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

// Import user events into a catalog from BigQuery using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using System;

/// <summary>
/// The import user events BigQuery sample class.
/// </summary>
public class ImportUserEventsBigQuerySample
{
    /// <summary>
    /// Get import user events BigQuery request.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The import user events request.</returns>
    public static ImportUserEventsRequest GetImportUserEventsBigQueryRequest(string projectId)
    {
        string dataSetId = "user_events";
        string dataSchema = "user_event";
        string tableId = "events";
        // To check error handling use the table of invalid user events:
        // tableId = "events_some_invalid";

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
                BigQuerySource = new BigQuerySource
                {
                    ProjectId = projectId,
                    DatasetId = dataSetId,
                    TableId = tableId,
                    DataSchema = dataSchema
                }
            }
        };

        Console.WriteLine("Import user events from BigQuery source. request:");
        Console.WriteLine(importRequest);
        Console.WriteLine();

        return importRequest;
    }

    /// <summary>
    /// Call the Retail API to import user events.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    public static Operation<ImportUserEventsResponse, ImportMetadata> ImportProductsFromBigQuery(string projectId)
    {
        ImportUserEventsRequest importBigQueryRequest = GetImportUserEventsBigQueryRequest(projectId);
        UserEventServiceClient client = UserEventServiceClient.Create();
        Operation<ImportUserEventsResponse, ImportMetadata> importResponse = client.ImportUserEvents(importBigQueryRequest);

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
/// The import user events BigQuery tutorial class.
/// </summary>
public static class ImportUserEventsBigQueryTutorial
{
    [Runner.Attributes.Example]
    public static Operation<ImportUserEventsResponse, ImportMetadata> ImportProductsFromBigQuery()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        var result = ImportUserEventsBigQuerySample.ImportProductsFromBigQuery(projectId);

        return result;
    }
}
