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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Class that performs creeation of all necessary test resources.
/// </summary>
public class CreateTestResources
{
    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    private static readonly StorageClient storageClient = StorageClient.Create();
    private static readonly BigQueryClient bigQueryClient = BigQueryClient.Create(projectId);

    /// <summary>
    /// Get the current solution directory full name.
    /// </summary>
    /// <param name="currentPath">The current path.</param>
    /// <returns>Full name of the current solution directory.</returns>
    public static string GetSolutionDirectoryFullName(string currentPath = null)
    {
        var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());

        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return directory.FullName;
    }

    /// <summary>Create GCS bucket.</summary>
    public static Bucket CreateBucket(string bucketName)
    {
        var newBucket = new Bucket();
        Console.WriteLine($"\nBucket name: {bucketName}\n");

        var bucketExists = CheckIfBucketExists(bucketName);

        if (bucketExists)
        {
            Console.WriteLine($"\nBucket {bucketName} already exists.\n");
            return storageClient.GetBucket(bucketName);
        }
        else
        {
            var bucket = new Bucket
            {
                Name = bucketName,
                StorageClass = "STANDARD",
                Location = "us"
            };

            newBucket = storageClient.CreateBucket(projectId, bucket);

            Console.WriteLine($"\nCreated bucket {newBucket.Name} in {newBucket.Location} with storage class {newBucket.StorageClass}\n");
        };

        return newBucket;
    }

    public static bool CheckIfBucketExists(string newBucketName)
    {
        var bucketExists = false;
        var bucketsInYourProject = ListBuckets();
        var bucketNamesInYourProject = bucketsInYourProject.Select(x => x.Name).ToArray();

        foreach (var existingBucketName in bucketNamesInYourProject)
        {
            if (existingBucketName == newBucketName)
            {
                bucketExists = true;
                break;
            }
        }

        return bucketExists;
    }

    /// <summary>List all existing buckets.</summary>
    public static IEnumerable<Bucket> ListBuckets() =>
        storageClient.ListBuckets(projectId);

    /// <summary>Upload blob.</summary>
    public static void UploadBlob(string bucketName, string localPath, string objectName)
    {
        using var fileStream = File.OpenRead(localPath);
        storageClient.UploadObject(bucketName, objectName, null, fileStream);
        Console.WriteLine($"Uploaded {objectName}.");
    }

    /// <summary>Get import products GCS request.</summary>
    public static ImportProductsRequest GetImportProductsGcsRequest(string gcsObjectName, string productsBucketName)
    {
        string gcsBucket = $"gs://{productsBucketName}";
        string gcsErrorsBucket = $"{gcsBucket}/error";

        var gcsSource = new GcsSource();
        gcsSource.InputUris.Add($"{gcsBucket}/{gcsObjectName}");

        var inputConfig = new ProductInputConfig
        {
            GcsSource = gcsSource
        };

        Console.WriteLine("\nGCS source: \n" + gcsSource.InputUris);

        var errorsConfig = new ImportErrorsConfig
        {
            GcsPrefix = gcsErrorsBucket
        };

        string locationId = "global";
        string catalogId = "default_catalog";
        string branchId = "0";
        BranchName defaultBranch = new BranchName(projectId, locationId, catalogId, branchId);

        var importRequest = new ImportProductsRequest
        {
            ParentAsBranchName = defaultBranch,
            ReconciliationMode = ImportProductsRequest.Types.ReconciliationMode.Incremental,
            InputConfig = inputConfig,
            ErrorsConfig = errorsConfig
        };

        Console.WriteLine("\nImport products from google cloud source. request: \n");
        Console.WriteLine($"Parent: {importRequest.Parent}");
        Console.WriteLine($"ReconciliationMode: {importRequest.ReconciliationMode}");
        Console.WriteLine($"InputConfig: {importRequest.InputConfig}");
        Console.WriteLine($"ErrorsConfig: {importRequest.ErrorsConfig}");

        return importRequest;
    }

    /// <summary>Call the Retail API to import products.</summary>
    public static void ImportProductsFromGcs(string productsBucketName, string productFileName)
    {
        var importGcsRequest = GetImportProductsGcsRequest(productFileName, productsBucketName);

        var client = ProductServiceClient.Create();
        var importResponse = client.ImportProducts(importGcsRequest);

        Console.WriteLine("\nThe operation was started: \n" + importResponse.Name);
        Console.WriteLine("\nPlease wait till opeartion is done");

        var importResult = importResponse.PollUntilCompleted();

        Console.WriteLine("Import products operation is done\n");
        Console.WriteLine("Number of successfully imported products: " + importResult.Metadata.SuccessCount);
        Console.WriteLine("Number of failures during the importing: " + importResult.Metadata.FailureCount);
        Console.WriteLine("\nOperation result: \n" + importResult.Result);

        // The imported products needs to be indexed in the catalog before they become available for search.
        Console.WriteLine("Wait 2 - 5 minutes till products become indexed in the catalog, after that they will be available for search");
    }

    /// <summary>Create a Big Query Dataset.</summary>
    public static void CreateBQDataSet(string dataSetName)
    {
        string fullDataSetId = $"{projectId}.{dataSetName}";
        Console.WriteLine($"Creating dataset {fullDataSetId}");

        try
        {
            DatasetReference datasetReference = new DatasetReference
            {
                DatasetId = dataSetName,
                ProjectId = projectId
            };

            var dataset = bigQueryClient.GetDataset(datasetReference);
            Console.WriteLine($"Dataset {fullDataSetId} already exists");
        }
        catch (Exception)
        {
            Dataset dataset = new Dataset
            {
                Location = "US"
            };

            // Construct a Dataset object to send to the API.
            BigQueryDataset dataSet = bigQueryClient.CreateDataset(dataSetName, dataset);
            Console.WriteLine($"Dataset {fullDataSetId} created");
        };
    }

    /// <summary>Create a Big Query Table.</summary>
    public static void CreateAndPopulateBQTable(string dataSetName, string tableName, string tableSchemaFilePath, string tableDataFilePath)
    {
        string fullTableId = $"{projectId}.{dataSetName}.{tableName}";
        Console.WriteLine($"Check if BQ table {fullTableId} exists");

        TableReference tableReference = new TableReference
        {
            TableId = tableName,
            DatasetId = dataSetName,
            ProjectId = projectId
        };

        try
        {
            BigQueryTable tableToDelete = bigQueryClient.GetTable(tableReference);

            Console.WriteLine($"Table {tableToDelete.FullyQualifiedId} exists and will be deleted");

            bigQueryClient.DeleteTable(tableReference);

            Console.WriteLine($"Table {tableToDelete.FullyQualifiedId} was deleted.");
        }
        catch (Exception)
        {
            Console.WriteLine($"Table {fullTableId} does not exist.");
        }

        Console.WriteLine($"Creating BigQuery Table {fullTableId}");

        TableSchemaBuilder tableSchemaBuilder = new TableSchemaBuilder();

        // Parsing json schema.
        string jsonSchema = File.ReadAllText(tableSchemaFilePath);

        JsonTextReader jsonReader = new JsonTextReader(new StringReader(jsonSchema))
        {
            SupportMultipleContent = true
        };

        JsonSerializer jsonSerializer = new JsonSerializer();

        while (jsonReader.Read())
        {
            TableFieldSchema tableFieldSchema = jsonSerializer.Deserialize<TableFieldSchema>(jsonReader);

            tableSchemaBuilder.Add(tableFieldSchema);
        }

        TableSchema finalTableSchema = tableSchemaBuilder.Build();

        // Creating a BigQuery table.
        try
        {
            BigQueryTable createdBigQueryTable = bigQueryClient.CreateTable(tableReference, finalTableSchema);

            Console.WriteLine($"Created BigQuery Table {createdBigQueryTable.FullyQualifiedId}");

            try
            {
                // Uploading json data to BigQuery table.
                Console.WriteLine($"Uploading data from json to BigQuery table {createdBigQueryTable.FullyQualifiedId}");

                using (FileStream sourceStream = File.Open(tableDataFilePath, FileMode.Open))
                {
                    BigQueryJob bigQueryJob = bigQueryClient.UploadJson(tableReference, finalTableSchema, sourceStream);

                    var result = bigQueryJob.PollUntilCompleted();
                }

                BigQueryTable createdTable = bigQueryClient.GetTable(tableReference);

                Console.WriteLine($"Uploaded {createdTable.Resource.NumRows} rows to {createdTable.FullyQualifiedId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Table {createdBigQueryTable.FullyQualifiedId} was not populated with data. Error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Table {tableName} was not created. Error: {ex.Message}");
        }
    }
}

/// <summary>
/// Create test resources.
/// </summary>

public static class CreateTestResourcesTutorial
{
    private static readonly string ProductFileName = "products.json";
    private static readonly string EventsFileName = "user_events.json";

    private static readonly string ProductDataSet = "products";
    private static readonly string ProductTable = "products";
    private static readonly string EventsDataSet = "user_events";
    private static readonly string EventsTable = "events";

    private static readonly string productsBucketName = Environment.GetEnvironmentVariable("RETAIL_BUCKET_NAME");
    private static readonly string eventsBucketName = Environment.GetEnvironmentVariable("RETAIL_EVENTS_BUCKET_NAME");

    private static readonly string productFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{ProductFileName}");
    private static readonly string eventsFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{EventsFileName}");

    private static readonly string ProductSchema = "product_schema.json";
    private static readonly string EventsSchema = "events_schema.json";

    private static readonly string productSchemaFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{ProductSchema}");
    private static readonly string eventsSchemaFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{EventsSchema}");

    [Runner.Attributes.Example]
    public static void PerformCreationOfTestResources()
    {
        // Create a GCS bucket with products.json file.
        var createdProductsBucket = CreateTestResources.CreateBucket(productsBucketName);
        CreateTestResources.UploadBlob(createdProductsBucket.Name, productFilePath, ProductFileName);

        // Create a GCS bucket with user_events.json file.
        var createdEventsBucket = CreateTestResources.CreateBucket(eventsBucketName);
        CreateTestResources.UploadBlob(createdEventsBucket.Name, eventsFilePath, EventsFileName);

        // Import products from the GCS bucket to the Retail catalog.
        CreateTestResources.ImportProductsFromGcs(productsBucketName, ProductFileName);

        // Create a BigQuery table with products.
        CreateTestResources.CreateBQDataSet(ProductDataSet);
        CreateTestResources.CreateAndPopulateBQTable(ProductDataSet, ProductTable, productSchemaFilePath, productFilePath);

        // Create a BigQuery table with user events.
        CreateTestResources.CreateBQDataSet(EventsDataSet);
        CreateTestResources.CreateAndPopulateBQTable(EventsDataSet, EventsTable, eventsSchemaFilePath, eventsFilePath);
    }
}