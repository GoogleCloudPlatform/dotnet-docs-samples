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
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

public static class ProductsCreateBigQueryTable
{
    private const string ProductFileName = "products.json";
    private const string ProductDataSet = "products";
    private const string ProductTable = "products";
    private const string ProductSchema = "product_schema.json";
    private const string InvalidProductTable = "products_some_invalid";

    private static readonly string productFilePath = Path.Combine(GetSolutionDirectoryFullName(), $"RetailProducts.Samples/resources/{ProductFileName}");
    private static readonly string productSchemaFilePath = Path.Combine(GetSolutionDirectoryFullName(), $"RetailProducts.Samples/resources/{ProductSchema}");

    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

    private static readonly BigQueryClient bigQueryClient = BigQueryClient.Create(projectId);

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
    private static void CreateAndPopulateBQTable(string dataSetName, string tableName, string tableSchemaFilePath, string tableDataFilePath)
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

    /// <summary>
    /// Create big query table.
    /// </summary>
    public static void PerformCreationOfBigQueryTable()
    {
        CreateBQDataSet(ProductDataSet);
        CreateAndPopulateBQTable(ProductDataSet, ProductTable, productSchemaFilePath, productFilePath);
    }
}