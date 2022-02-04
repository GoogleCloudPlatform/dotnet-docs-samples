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

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Retail.V2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

/// <summary>
/// Class that performs creeation of all necessary test resources.
/// </summary>
public static class CreateTestResources
{
    private const string ProductFileName = "products.json";
    private const string ProductsObject = "products.json";

    private const string EventsFileName = "user_events.json";

    private const string DataSetId = "products";

    private const string WindowsTerminalName = "cmd.exe";
    private const string UnixTerminalName = "/bin/bash";
    private const string WindowsTerminalPrefix = "/c ";
    private const string UnixTerminalPrefix = "-c ";
    private const string WindowsTerminalQuotes = "";
    private const string UnixTerminalQuotes = "\"";

    private const string ProductDataSet = "products";
    private const string ProductTable = "products";
    private const string ProductSchema = "resources/product_schema.json";
    private const string EventsDataSet = "user_events";
    private const string EventsTable = "events";
    private const string EventsSchema = "resources/events_schema.json";

    private static readonly bool CurrentOSIsWindows = Environment.OSVersion.VersionString.Contains("Windows");
    private static readonly string CurrentTerminalPrefix = CurrentOSIsWindows ? WindowsTerminalPrefix : UnixTerminalPrefix;
    private static readonly string CurrentTerminalFile = CurrentOSIsWindows ? WindowsTerminalName : UnixTerminalName;
    private static readonly string CurrentTerminalQuotes = CurrentOSIsWindows ? WindowsTerminalQuotes : UnixTerminalQuotes;

    private static readonly string productFilePath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), $"TestResourcesSetupCleanup/resources/{ProductFileName}");
    private static readonly string eventsFilePath = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), $"TestResourcesSetupCleanup/resources/{EventsFileName}");
    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID");
    private static readonly string projectNumber = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_NUMBER");
    private static readonly string bucketName = Environment.GetEnvironmentVariable("BUCKET_NAME");

    private static readonly string defaultCatalog = $"projects/{projectNumber}/locations/global/catalogs/default_catalog/branches/default_branch";
    private static readonly string gcsBucket = $"gs://{bucketName}";
    private static readonly string gcsErrorsBucket = $"{gcsBucket}/error";

    private static readonly StorageClient storageClient = StorageClient.Create();

    /// <summary>Create GCS bucket.</summary>
    private static Bucket CreateBucket(string bucketName)
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

    private static bool CheckIfBucketExists(string newBucketName)
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
    private static List<Bucket> ListBuckets()
    {
        var bucketsList = new List<Bucket>();
        var buckets = storageClient.ListBuckets(projectId);

        foreach (var bucket in buckets)
        {
            bucketsList.Add(bucket);
            Console.WriteLine(bucket.Name);
        }

        return bucketsList;
    }

    /// <summary>Upload blob.</summary>
    private static void UploadBlob(string bucketName, string localPath, string objectName)
    {
        var bucket = storageClient.GetBucket(bucketName);

        using var fileStream = File.OpenRead(localPath);
        storageClient.UploadObject(bucketName, objectName, null, fileStream);
        Console.WriteLine($"Uploaded {objectName}.");
    }

    /// <summary>Get import products gcs request.</summary>
    private static ImportProductsRequest GetImportProductsGcsRequest(string gcsObjectName)
    {
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

        // To check error handling paste the invalid catalog name here:
        // defaultCatalog = "invalid_catalog_name"
        var importRequest = new ImportProductsRequest
        {
            Parent = defaultCatalog,
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
    public static void ImportProductsFromGcs()
    {
        var importGcsRequest = GetImportProductsGcsRequest(ProductsObject);

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
    private static void CreateBQDataSet(string dataSetName)
    {
        var listDataSets = ListBQDataSets();
        if (!listDataSets.Contains(dataSetName))
        {
            string createDataSetCommand = CurrentTerminalPrefix + CurrentTerminalQuotes + $"bq --location=US mk -d --default_table_expiration 3600 --description \"This is my dataset.\" {projectId}:{dataSetName}" + CurrentTerminalQuotes;
            string consoleOutput = string.Empty;

            var processStartInfo = new ProcessStartInfo(CurrentTerminalFile, createDataSetCommand)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.Start();

                consoleOutput = process.StandardOutput.ReadToEnd();
            }
        }
        else
        {
            Console.WriteLine($"Dataset {dataSetName} already exists.");
        }
    }

    /// <summary>List Big Query Datasets.</summary>
    private static string ListBQDataSets()
    {
        string dataSets = string.Empty;

        string listDataSetCommand = CurrentTerminalPrefix + CurrentTerminalQuotes + $"bq ls --project_id {projectId}" + CurrentTerminalQuotes;

        var processStartInfo = new ProcessStartInfo(CurrentTerminalFile, listDataSetCommand)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process())
        {
            process.StartInfo = processStartInfo;

            process.Start();

            dataSets = process.StandardOutput.ReadToEnd();

            Console.WriteLine(dataSets);
        }

        return dataSets;
    }

    /// <summary>Create a Big Query Table.</summary>
    private static void CreateBQTable(string dataSet, string tableName, string schema)
    {
        var listBQTables = ListBQTables(dataSet);
        Console.WriteLine($"Creating BigQuery table {tableName}");

        if (!listBQTables.Contains(tableName))
        {
            string consoleOutput = string.Empty;

            var createTableCommand = CurrentTerminalPrefix + CurrentTerminalQuotes + $"bq mk --table {projectId}:{dataSet}.{tableName} {schema}" + CurrentTerminalQuotes;

            var procStartInfo = new ProcessStartInfo(CurrentTerminalFile, createTableCommand)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = procStartInfo;
                process.Start();

                consoleOutput = process.StandardOutput.ReadToEnd();
                Console.WriteLine(consoleOutput);
            }
        }
        else
        {
            Console.WriteLine($"Table {tableName} already exists.");
        }

    }

    /// <summary>List Big Query Tables.</summary>
    private static string ListBQTables(string dataSet)
    {
        string tables = string.Empty;
        var listTablesCommand = CurrentTerminalPrefix + CurrentTerminalQuotes + $"bq ls {projectId}:{dataSet}" + CurrentTerminalQuotes;

        var procStartInfo = new ProcessStartInfo(CurrentTerminalFile, listTablesCommand)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = procStartInfo;
            process.Start();

            tables = process.StandardOutput.ReadToEnd();
            Console.WriteLine("Tables: \n" + tables);

            return tables;
        }
    }

    /// <summary>Upload data to Big Query Table.</summary>
    private static void UploadDataToBQTable(string dataSet, string tableName, string source, string schema)
    {
        string consoleOutput = string.Empty;
        Console.WriteLine($"Uploading data from {source} to the table {dataSet}.{tableName}");

        var uploadDataCommand = CurrentTerminalPrefix + CurrentTerminalQuotes + $"bq load --source_format=NEWLINE_DELIMITED_JSON {projectId}:{dataSet}.{tableName} {source} {schema}" + CurrentTerminalQuotes;

        var procStartInfo = new ProcessStartInfo(CurrentTerminalFile, uploadDataCommand)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = procStartInfo;
            process.Start();

            consoleOutput = process.StandardOutput.ReadToEnd();
            Console.WriteLine(consoleOutput);
        }
    }

    /// <summary>
    /// Create test resources.
    /// </summary>
    [TestResourcesSetupCleanup.Attributes.Example]
    public static void PerformCreationOfTestResources()
    {
        // Create a GCS bucket with products.json file.
        var createdProductsBucket = CreateBucket(bucketName);
        UploadBlob(createdProductsBucket.Name, productFilePath, ProductFileName);
        
        // Create a GCS bucket with user_events.json file.
        var createdEventsBucket = CreateBucket(bucketName);
        UploadBlob(createdEventsBucket.Name, eventsFilePath, EventsFileName);
        
        // Import products from the GCS bucket to the Retail catalog.
        ImportProductsFromGcs();

        // Create a BigQuery table with products.
        CreateBQDataSet(ProductDataSet);
        CreateBQTable(ProductDataSet, ProductTable, ProductSchema);
        UploadDataToBQTable(ProductDataSet, ProductTable, productFilePath, ProductSchema);

        // Create a BigQuery table with user events.
        CreateBQDataSet(EventsDataSet);
        CreateBQTable(EventsDataSet, EventsTable, EventsSchema);
        UploadDataToBQTable(EventsDataSet, EventsTable, eventsFilePath, EventsSchema);
    }
}