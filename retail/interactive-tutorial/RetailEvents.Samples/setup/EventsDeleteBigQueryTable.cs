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

using Google.Cloud.Retail.V2;
using System;
using System.Diagnostics;

public static class EventsDeleteBigQueryTable
{
    private const string WindowsTerminalName = "cmd.exe";
    private const string UnixTerminalName = "/bin/bash";
    private const string WindowsTerminalPrefix = "/c ";
    private const string UnixTerminalPrefix = "-c ";
    private const string WindowsTerminalQuotes = "";
    private const string UnixTerminalQuotes = "\"";

    private const string EventsDataSet = "user_events";

    private static readonly bool CurrentOSIsWindows = Environment.OSVersion.VersionString.Contains("Windows");
    private static readonly string CurrentTerminalPrefix = CurrentOSIsWindows ? WindowsTerminalPrefix : UnixTerminalPrefix;
    private static readonly string CurrentTerminalFile = CurrentOSIsWindows ? WindowsTerminalName : UnixTerminalName;
    private static readonly string CurrentTerminalQuotes = CurrentOSIsWindows ? WindowsTerminalQuotes : UnixTerminalQuotes;

    private static readonly string projectNumber = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_NUMBER");

    private static readonly string defaultCatalog = $"projects/{projectNumber}/locations/global/catalogs/default_catalog/branches/default_branch";

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
                Console.WriteLine("Ignore PermissionDenied in case the product does not exist at time of deletion");
            }
        }

        Console.WriteLine($"{deleteCount} products were deleted from {defaultCatalog}");
    }

    /// <summary>Delete Big Query dataset with tables.</summary>
    private static void DeleteBQDatasetWithTables(string dataset)
    {
        string consoleOutput = string.Empty;
        Console.WriteLine("Deleting a BigQuery dataset with all tables");

        var deleteDatasetCommand = CurrentTerminalPrefix + CurrentTerminalQuotes + $"bq rm -r -d -f {dataset}" + CurrentTerminalQuotes;

        var procStartInfo = new ProcessStartInfo(CurrentTerminalFile, deleteDatasetCommand)
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
    /// Delete test resources.
    /// </summary>
    public static void PerformDeletionOfEventsBigQueryTable()
    {
        DeleteAllProducts();
        DeleteBQDatasetWithTables(EventsDataSet);
    }
}