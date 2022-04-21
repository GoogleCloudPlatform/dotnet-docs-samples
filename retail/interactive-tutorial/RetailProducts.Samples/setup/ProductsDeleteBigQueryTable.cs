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

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Retail.V2;
using System;

public static class ProductsDeleteBigQueryTable
{
    private const string ProductDataSet = "products";
    private const string ProductTable = "products";

    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private static readonly string defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/0";

    private static readonly BigQueryClient bigQueryClient = BigQueryClient.Create(projectId);

    /// <summary>Delete all products.</summary>
    private static void DeleteAllProducts()
    {
        Console.WriteLine($"Deleting all products from catalog, please wait.");

        var productServiceClient = ProductServiceClient.Create();
        var listProductsRequest = new ListProductsRequest
        {
            Parent = defaultCatalog
        };

        var products = productServiceClient.ListProducts(listProductsRequest);
        int deleteCount = 0;

        foreach (var product in products)
        {
            var deleteProductRequest = new DeleteProductRequest
            {
                Name = product.Name
            };

            try
            {
                productServiceClient.DeleteProduct(deleteProductRequest);
                deleteCount++;
            }
            catch (Exception)
            {
                Console.WriteLine("Ignore PermissionDenied in case the product does not exist at time of deletion.");
            }
        }

        Console.WriteLine($"{deleteCount} products were deleted from {defaultCatalog}");
    }

    /// <summary>Delete Big Query dataset with tables.</summary>
    public static void DeleteBQDatasetWithData(string datasetId)
    {
        Console.WriteLine($"Deleting a {datasetId} BigQuery dataset with all its contents.");

        DatasetReference datasetReference = new DatasetReference
        {
            DatasetId = datasetId,
            ProjectId = projectId
        };

        DeleteDatasetOptions deleteOptions = new DeleteDatasetOptions
        {
            DeleteContents = true
        };

        try
        {
            bigQueryClient.DeleteDataset(datasetReference, deleteOptions);

            Console.WriteLine($"Dataset {datasetId} and contents were deleted.");
        }
        catch (Exception)
        {
            Console.WriteLine($"Dataset {datasetId} does not exist.");
        }
    }

    /// <summary>Delete Big Query dataset with tables.</summary>
    public static void DeleteBQTable(string datasetId, string tableId)
    {
        Console.WriteLine($"Deleting a {tableId} BigQuery table.");

        TableReference tableReference = new TableReference
        {
            TableId = tableId,
            DatasetId = datasetId,
            ProjectId = projectId
        };

        try
        {
            bigQueryClient.DeleteTable(tableReference);

            Console.WriteLine($"Table {tableId} was deleted.");
        }
        catch (Exception)
        {
            Console.WriteLine($"Table {tableId} does not exist.");
        }
    }

    /// <summary>
    /// Delete test resources.
    /// </summary>
    public static void PerformDeletionOfProductsBigQueryTable()
    {
        DeleteAllProducts();
        DeleteBQDatasetWithData(ProductDataSet);
        DeleteBQTable(ProductDataSet, ProductTable);
    }
}