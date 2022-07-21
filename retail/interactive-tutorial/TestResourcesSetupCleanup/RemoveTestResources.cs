// Copyright 2022 Google Inc. All Rights Reserved.
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
using Google.Apis.Storage.v1.Data;
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Retail.V2;
using Google.Cloud.Storage.V1;
using System;

/// <summary>
/// Class that performs creeation of all necessary test resources.
/// </summary>
public class RemoveTestResources
{
    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    private static readonly StorageClient storageClient = StorageClient.Create();
    private static readonly BigQueryClient bigQueryClient = BigQueryClient.Create(projectId);

    /// <summary>Delete bucket.</summary>
    public static void DeleteBucket(string bucketName)
    {
        Console.WriteLine($"Deleting Bucket {bucketName}.");

        try
        {
            var bucketToDelete = storageClient.GetBucket(bucketName);
            DeleteObjectsFromBucket(bucketToDelete);
            storageClient.DeleteBucket(bucketToDelete.Name);
            Console.WriteLine($"Bucket {bucketToDelete.Name} is deleted.");
        }
        catch (Exception)
        {
            Console.WriteLine($"Bucket {bucketName} does not exist.");
        }
    }

    /// <summary>Delete all objects from bucket.</summary>
    public static void DeleteObjectsFromBucket(Bucket bucket)
    {
        Console.WriteLine($"Deleting object from bucket {bucket.Name}.");

        var blobs = storageClient.ListObjects(bucket.Name);
        foreach (var blob in blobs)
        {
            storageClient.DeleteObject(bucket.Name, blob.Name);
        }

        Console.WriteLine($"All objects are deleted from a GCS bucket {bucket.Name}.");
    }

    /// <summary>Delete all products.</summary>
    public static void DeleteAllProducts()
    {
        Console.WriteLine($"Deleting all products from catalog, please wait.");

        string locationId = "global";
        string catalogId = "default_catalog";
        string branchId = "0";
        BranchName defaultBranch = new BranchName(projectId, locationId, catalogId, branchId);

        var productServiceClient = ProductServiceClient.Create();
        var listProductsRequest = new ListProductsRequest
        {
            ParentAsBranchName = defaultBranch,
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

        Console.WriteLine($"{deleteCount} products were deleted from {defaultBranch}");
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
}
/// <summary>
/// Delete test resources.
/// </summary>
public static class RemoveTestResourcesTutorial
{
    private const string ProductsDataSet = "products";
    private const string EventsDataSet = "user_events";
    private const string ProductsTable = "products";
    private const string EventsTable = "events";

    private static readonly string productsBucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");
    private static readonly string eventsBucketName = Environment.GetEnvironmentVariable("EVENTS_BUCKET_NAME");

    [Runner.Attributes.Example]
    public static void PerformDeletionOfTestResources()
    {
        // Delete products and events GCS buckets
        RemoveTestResources.DeleteBucket(productsBucketName);
        RemoveTestResources.DeleteBucket(eventsBucketName);

        // Delete all products from the Retail catalog
        // DeleteAllProducts();

        // Delete events and products BQ tables
        RemoveTestResources.DeleteBQTable(ProductsDataSet, ProductsTable);
        RemoveTestResources.DeleteBQTable(EventsDataSet, EventsTable);

        // Delete products and events datasets
        RemoveTestResources.DeleteBQDatasetWithData(ProductsDataSet);
        RemoveTestResources.DeleteBQDatasetWithData(EventsDataSet);

        // Delete all products from the Retail catalog
        RemoveTestResources.DeleteAllProducts();
    }
}