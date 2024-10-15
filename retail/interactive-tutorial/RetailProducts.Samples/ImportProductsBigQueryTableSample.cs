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

// Import products into a catalog from BigQuery table using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using System;

/// <summary>
/// The import products BigQuery table sample class.
/// </summary>
public class ImportProductsBigQueryTableSample
{
    /// <summary>
    /// Get import products BigQuery request.
    /// </summary>
    /// <param name="reconciliationMode">The preffered reconciliation mode.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The import product request.</returns>
    public static ImportProductsRequest GetImportProductsBigQueryRequest(ImportProductsRequest.Types.ReconciliationMode reconciliationMode, string projectId)
    {
        string dataSetId = "products";
        string tableId = "products";
        // To check error handling use the table of invalid products:
        // tableId = "products_some_invalid";

        string locationId = "global";
        string catalogId = "default_catalog";
        string branchId = "default_branch";
        // To check error handling paste the invalid catalog name here:
        // catalogId = "invalid_catalog_name";
        BranchName defaultBranch = new BranchName(projectId, locationId, catalogId, branchId);

        var importRequest = new ImportProductsRequest
        {
            ParentAsBranchName = defaultBranch,
            ReconciliationMode = reconciliationMode,
            InputConfig = new ProductInputConfig
            {
                BigQuerySource = new BigQuerySource
                {
                    ProjectId = projectId,
                    DatasetId = dataSetId,
                    TableId = tableId
                }
            }
        };

        Console.WriteLine("Import products from BigQuery table request:");
        Console.WriteLine(importRequest);
        Console.WriteLine();

        return importRequest;
    }

    /// <summary>
    /// Call the Retail API to import products from a BigQuery.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    public static Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromBigQuery(string projectId)
    {
        // Try the full reconciliation mode here:
        var recoinciliationMode = ImportProductsRequest.Types.ReconciliationMode.Incremental;
        ImportProductsRequest importBigQueryRequest = GetImportProductsBigQueryRequest(recoinciliationMode, projectId);
        ProductServiceClient client = ProductServiceClient.Create();
        Operation<ImportProductsResponse, ImportMetadata> importResponse = client.ImportProducts(importBigQueryRequest);

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

        return importResult;
    }
}

/// <summary>
/// The import products BigQuery table tutorial class.
/// </summary>
public static class ImportProductsBigQueryTableTutorial
{
    [Runner.Attributes.Example]
    public static Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromBigQuery()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        var result = ImportProductsBigQueryTableSample.ImportProductsFromBigQuery(projectId);

        return result;
    }
}
