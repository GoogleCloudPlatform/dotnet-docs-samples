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

// [START retail_import_products_from_gcs]
// Import products into a catalog from gcs using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using System;

/// <summary>
/// The import products gcs sample class.
/// </summary>
public class ImportProductsGcsSample
{
    private const string gcsProductsObject = "products.json";

    // To check error handling use the json with invalid product:
    // gcsProductsObject = "products_some_invalid.json";

    /// <summary>
    /// Get import products gcs request.
    /// </summary>
    /// <param name="gcsObjectName">The name of the gcs object.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The import products request.</returns>
    private static ImportProductsRequest GetImportProductsGcsRequest(string gcsObjectName, string projectId, string bucketName)
    {
        string productsBucketName = bucketName ?? Environment.GetEnvironmentVariable("BUCKET_NAME");
        string gcsBucket = $"gs://{productsBucketName}";
        string gcsErrorsBucket = $"{gcsBucket}/error";

        string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch";

        // To check error handling paste the invalid catalog name here:
        // defaultCatalog = "invalid_catalog_name";

        var gcsSource = new GcsSource();
        gcsSource.InputUris.Add($"{gcsBucket}/{gcsObjectName}");

        Console.WriteLine("GCS source:");
        Console.WriteLine(gcsSource.InputUris);
        Console.WriteLine();

        var importRequest = new ImportProductsRequest
        {
            Parent = defaultCatalog,
            ReconciliationMode = ImportProductsRequest.Types.ReconciliationMode.Incremental,
            InputConfig = new ProductInputConfig
            {
                GcsSource = gcsSource
            },
            ErrorsConfig = new ImportErrorsConfig
            {
                GcsPrefix = gcsErrorsBucket
            }
        };

        Console.WriteLine("Import products from google cloud source. request:");
        Console.WriteLine(importRequest);
        Console.WriteLine();

        return importRequest;
    }

    /// <summary>
    /// Call the Retail API to import products.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    public Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromGcs(string projectId, string bucketName = null)
    {
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
        Console.WriteLine("Wait 2 - 5 minutes till products become indexed in the catalog, after that they will be available for search");

        return importResult;
    }
}
// [END retail_import_products_from_gcs]

/// <summary>
/// The import products gcs tutorial class.
/// </summary>
public static class ImportProductsGcsTutorial
{
    [Runner.Attributes.Example]
    public static Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromGcs()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new ImportProductsGcsSample();

        string createdBucketName = null;

        ProductsDeleteGcsBucket.PerformDeletionOfProductsGcsBucket(createdBucketName);
        createdBucketName = ProductsCreateGcsBucket.PerformCreationOfGcsBucket();

        Operation<ImportProductsResponse, ImportMetadata> importResponse = sample.ImportProductsFromGcs(projectId, createdBucketName);

        ProductsDeleteGcsBucket.PerformDeletionOfProductsGcsBucket(createdBucketName);

        return importResponse;
    }
}