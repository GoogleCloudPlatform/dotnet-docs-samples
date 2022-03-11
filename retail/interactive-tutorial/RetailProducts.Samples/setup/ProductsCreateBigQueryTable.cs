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

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;


public static class ProductsCreateBigQueryTable
{
    private const string ProductFileName = "products.json";

    private const string WindowsTerminalName = "cmd.exe";
    private const string UnixTerminalName = "/bin/bash";
    private const string WindowsTerminalPrefix = "/c ";
    private const string UnixTerminalPrefix = "-c ";
    private const string WindowsTerminalQuotes = "";
    private const string UnixTerminalQuotes = "\"";

    private const string ProductDataSet = "products";
    private const string ProductTable = "products";
    private const string ProductSchema = "resources/product_schema.json";
    private const string InvalidProductTable = "products_some_invalid";

    private static readonly bool CurrentOSIsWindows = Environment.OSVersion.VersionString.Contains("Windows");
    private static readonly string CurrentTerminalPrefix = CurrentOSIsWindows ? WindowsTerminalPrefix : UnixTerminalPrefix;
    private static readonly string CurrentTerminalFile = CurrentOSIsWindows ? WindowsTerminalName : UnixTerminalName;
    private static readonly string CurrentTerminalQuotes = CurrentOSIsWindows ? WindowsTerminalQuotes : UnixTerminalQuotes;

    private static readonly string productFilePath = Path.Combine(GetSolutionDirectoryFullName(), $"RetailProducts.Samples/resources/{ProductFileName}");
    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT_ID");

    /// <summary>
    /// Get the current solution directory full name.
    /// </summary>
    /// <param name="currentPath">The current path.</param>
    /// <returns>Full name of the current solution directory.</returns>
    private static string GetSolutionDirectoryFullName(string currentPath = null)
    {
        var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());

        while (directory != null && !directory.GetFiles("*.sln").Any())
        {
            directory = directory.Parent;
        }

        return directory.FullName;
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

        if (!listBQTables.Contains(dataSet))
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
    /// Create big query table.
    /// </summary>
    [Runner.Attributes.Example]
    public static void PerformCreationOfBigQueryTable()
    {
        CreateBQDataSet(ProductDataSet);
        CreateBQTable(ProductDataSet, ProductTable, ProductSchema);
        UploadDataToBQTable(ProductDataSet, ProductTable, productFilePath, ProductSchema);
        // CreateBQTable(ProductDataSet, InvalidProductTable, ProductSchema);
        // UploadDataToBQTable(ProductDataSet, InvalidProductTable, productFilePath, ProductSchema);
    }
}