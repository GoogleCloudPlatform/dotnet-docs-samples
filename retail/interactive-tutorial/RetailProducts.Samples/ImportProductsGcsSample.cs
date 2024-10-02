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

// Import products into a catalog from GCS using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using System;

/// <summary>
/// The import products GCS sample class.
/// </summary>
public class ImportProductsGcsSample
{
    /// <summary>
    /// Get import products GCS request.
    /// </summary>
    /// <param name="gcsObjectName">The name of the GCS object.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The import products request.</returns>
    public static ImportProductsRequest GetImportProductsGcsRequest(string gcsObjectName, string projectId, string productsBucketName)
    {
        string gcsBucket = $"gs://{productsBucketName}";
        string gcsErrorsPrefix = $"{gcsBucket}/error";

        string locationId = "global";
        string catalogId = "default_catalog";
        string branchId = "default_branch";
        // To check error handling paste the invalid catalog name here:
        // catalogId = "invalid_catalog_name";
        BranchName defaultBranch = new BranchName(projectId, locationId, catalogId, branchId);

        var importRequest = new ImportProductsRequest
        {
            ParentAsBranchName = defaultBranch,
            ReconciliationMode = ImportProductsRequest.Types.ReconciliationMode.Incremental,
            InputConfig = new ProductInputConfig
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

        Console.WriteLine("Import products from google cloud source request:");
        Console.WriteLine(importRequest);
        Console.WriteLine();

        return importRequest;
    }

    /// <summary>
    /// Call the Retail API to import products.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    public static Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromGcs(string projectId, string bucketName = null)
    {
        string gcsProductsObject = "products.json";
        // To check error handling use the json with invalid product:
        // gcsProductsObject = "products_some_invalid.json";

        ImportProductsRequest importGcsRequest = GetImportProductsGcsRequest(gcsProductsObject, projectId, bucketName);

        ProductServiceClient client = ProductServiceClient.Create();
        Operation<ImportProductsResponse, ImportMetadata> importResponse = client.ImportProducts(importGcsRequest);

        Console.WriteLine("The operation was started:");
        Console.WriteLine(importResponse.Name);
        Console.WriteLine();

        Console.WriteLine("Please wait till opeartion is done");
        Console.WriteLine();

        Operation<ImportProductsResponse, ImportMetadata> importResult = importResponse.PollUntilCompleted();

        Console.WriteLine("Import products operation is done");
        Console.WriteLine();

        Console.WriteLine("Number of successfully imported products: " + importResult.Metadata.SuccessCount);
        Console.WriteLine("Number of failures during the importing: " + importResult.Metadata.FailureCount);
        Console.WriteLine();
        Console.WriteLine("Operation result:");
        Console.WriteLine(importResult.Result);
        Console.WriteLine();

        // The imported products needs to be indexed in the catalog before they become available for search.
        Console.WriteLine("Wait few minutes till products become indexed in the catalog, after that they will be available for search");

        return importResult;
    }
}

/// <summary>
/// The import products GCS tutorial class.
/// </summary>
public static class ImportProductsGcsTutorial
{
    [Runner.Attributes.Example]
    public static Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromGcs()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        string productsBucketName = Environment.GetEnvironmentVariable("RETAIL_BUCKET_NAME");

        var result = ImportProductsGcsSample.ImportProductsFromGcs(projectId, productsBucketName);

        return result;
    }
}
