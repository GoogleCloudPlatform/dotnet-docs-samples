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

using System.IO;

public static class ProductsCreateBigQueryTable
{
    private const string ProductFileName = "products.json";
    private const string InvalidProductFileName = "products_some_invalid.json";
    private const string ProductDataSet = "products";
    private const string ProductTable = "products";
    private const string InvalidProductTable = "products_some_invalid";
    private const string ProductSchema = "product_schema.json";

    private static readonly string productFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{ProductFileName}");
    private static readonly string invalidProductFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{InvalidProductFileName}");
    private static readonly string productSchemaFilePath = Path.Combine(CreateTestResources.GetSolutionDirectoryFullName(), $"TestResourcesSetupCleanup/resources/{ProductSchema}");

    /// <summary>
    /// Create products BigQuery tables with data.
    /// </summary>
    [Runner.Attributes.Example]
    public static void PerformCreationOfBigQueryTable()
    {
        // Create a BigQuery tables with data.
        CreateTestResources.CreateBQDataSet(ProductDataSet);
        CreateTestResources.CreateAndPopulateBQTable(ProductDataSet, ProductTable, productSchemaFilePath, productFilePath);
        CreateTestResources.CreateAndPopulateBQTable(ProductDataSet, InvalidProductTable, productSchemaFilePath, invalidProductFilePath);
    }
}
