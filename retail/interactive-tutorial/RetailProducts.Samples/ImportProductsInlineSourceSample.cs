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

// Import products into a catalog from inline source using Retail API

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

/// <summary>
/// The import products inline source sample class.
/// </summary>
public class ImportProductsInlineSourceSample
{
    /// <summary>
    /// Generate two products for importing.
    /// </summary>
    /// <returns>List of two products.</returns>
    public static List<Product> GetProducts()
    {
        var products = new List<Product>();

        // First product data generation:
        var priceInfo1 = new PriceInfo
        {
            Price = 16.0f,
            OriginalPrice = 45.0f,
            Cost = 12.0f,
            CurrencyCode = "USD"
        };

        var colorInfo1 = new ColorInfo
        {
            ColorFamilies = { "Blue" },
            Colors = { "Light blue", "Blue", "Dark blue" }
        };

        var fulfillmentInfo1 = new FulfillmentInfo
        {
            Type = "pickup-in-store",
            PlaceIds = { "store1", "store2" },

        };

        var fieldMask1 = new FieldMask
        {
            Paths = { "title", "categories", "price_info", "color_info" }
        };

        // To check error handling comment out the product title here:
        var product1 = new Product
        {
            Title = "#IamRemarkable Pen",
            Id = Guid.NewGuid().ToString("N").Substring(0, 14),
            Uri = "https://shop.googlemerchandisestore.com/Google+Redesign/Office/IamRemarkable+Pen",
            PriceInfo = priceInfo1,
            ColorInfo = colorInfo1,
            RetrievableFields = fieldMask1,
            Categories = { "Office" },
            Brands = { "#IamRemarkable" },
            FulfillmentInfo = { fulfillmentInfo1 }
        };

        // Second product data generation:
        var priceInfo2 = new PriceInfo
        {
            Price = 35.0f,
            OriginalPrice = 45.0f,
            Cost = 12.0f,
            CurrencyCode = "USD"
        };

        var colorInfo2 = new ColorInfo
        {
            ColorFamilies = { "Blue" },
            Colors = { "Sky blue" }
        };

        var fulfillmentInfo2 = new FulfillmentInfo
        {
            Type = "pickup-in-store",
            PlaceIds = { "store2", "store3" }
        };

        var fieldMask2 = new FieldMask
        {
            Paths = { "title", "categories", "price_info", "color_info" }
        };

        // To check error handling comment out the product title here:
        var product2 = new Product
        {
            Title = "Android Embroidered Crewneck Sweater",
            Id = Guid.NewGuid().ToString("N").Substring(0, 14),
            Uri = "https://shop.googlemerchandisestore.com/Google+Redesign/Apparel/Android+Embroidered+Crewneck+Sweater",
            PriceInfo = priceInfo2,
            ColorInfo = colorInfo2,
            RetrievableFields = fieldMask2,
            Categories = { "Apparel" },
            Brands= { "Android" },
            FulfillmentInfo = { fulfillmentInfo2 }
        };

        products.Add(product1);
        products.Add(product2);

        return products;
    }

    /// <summary>
    /// Get import products inline request.
    /// </summary>
    /// <param name="productsToImport">The list of products to import.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The import products request.</returns>
    /// <summary>
    public static ImportProductsRequest GetImportProductsInlineRequest(List<Product> productsToImport, string projectId)
    {
        string locationId = "global";
        string catalogId = "default_catalog";
        string branchId = "default_branch";
        // To check error handling paste the invalid catalog name here:
        // catalogId = "invalid_catalog_name";
        BranchName defaultBranch = new BranchName(projectId, locationId, catalogId, branchId);

        var importRequest = new ImportProductsRequest
        {
            ParentAsBranchName = defaultBranch,
            InputConfig = new ProductInputConfig
            {
                ProductInlineSource = new ProductInlineSource
                {
                    Products = { productsToImport }
                }
            }
        };

        Console.WriteLine("Import products from inline source request:");
        Console.WriteLine(importRequest);
        Console.WriteLine();

        return importRequest;
    }

    /// <summary>
    /// Call the Retail API to import products.
    /// </summary>
    public static Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromInlineSource(string projectId)
    {
        List<Product> products = GetProducts();
        ImportProductsRequest importRequest = GetImportProductsInlineRequest(products, projectId);
        ProductServiceClient client = ProductServiceClient.Create();
        Operation<ImportProductsResponse, ImportMetadata> importResponse = client.ImportProducts(importRequest);

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
/// The import products inline source tutorial class.
/// </summary>
public static class ImportProductsInlineSourceTutorial
{
    [Runner.Attributes.Example]
    public static Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromInlineSource()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        var result =  ImportProductsInlineSourceSample.ImportProductsFromInlineSource(projectId);

        return result;
    }
}
