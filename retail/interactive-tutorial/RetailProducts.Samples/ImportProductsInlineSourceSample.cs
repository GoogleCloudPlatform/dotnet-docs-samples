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

// [START retail_import_products_from_inline_source]
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
    private static List<Product> GetProducts()
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

        string[] colors1 = { "Light blue", "Blue", "Dark blue"};
        var colorInfo1 = new ColorInfo();
        colorInfo1.ColorFamilies.Add("Blue");
        colorInfo1.Colors.AddRange(colors1);

        var fulfillmentInfo1 = new FulfillmentInfo();
        fulfillmentInfo1.Type = "pickup-in-store";
        string[] placeIds1 = { "store1", "store2" };
        fulfillmentInfo1.PlaceIds.AddRange(placeIds1);

        string[] paths1 = { "title", "categories", "price_info", "color_info" };
        var fieldMask1 = new FieldMask();
        fieldMask1.Paths.AddRange(paths1);

        // To check error handling comment out the product title here:
        var product1 = new Product
        {
            Title = "#IamRemarkable Pen",
            Id = Guid.NewGuid().ToString("N").Substring(0, 14),
            Uri = "https://shop.googlemerchandisestore.com/Google+Redesign/Office/IamRemarkable+Pen",
            PriceInfo = priceInfo1,
            ColorInfo = colorInfo1,
            RetrievableFields = fieldMask1,
        };

        product1.Categories.Add("Office");
        product1.Brands.Add("#IamRemarkable");
        product1.FulfillmentInfo.Add(fulfillmentInfo1);

        // Second product data generation:
        var priceInfo2 = new PriceInfo
        {
            Price = 35.0f,
            OriginalPrice = 45.0f,
            Cost = 12.0f,
            CurrencyCode = "USD"
        };

        string[] colors2 = { "Sky blue" };
        var colorInfo2 = new ColorInfo();
        colorInfo2.ColorFamilies.Add("Blue");
        colorInfo2.Colors.AddRange(colors2);

        var fulfillmentInfo2 = new FulfillmentInfo
        {
            Type = "pickup-in-store"
        };
        string[] placeIds2 = { "store2", "store3" };
        fulfillmentInfo2.PlaceIds.AddRange(placeIds2);

        string[] paths2 = { "title", "categories", "price_info", "color_info" };
        var fieldMask2 = new FieldMask();
        fieldMask2.Paths.AddRange(paths2);

        // To check error handling comment out the product title here:
        var product2 = new Product
        {
            Title = "Android Embroidered Crewneck Sweater",
            Id = Guid.NewGuid().ToString("N").Substring(0, 14),
            Uri = "https://shop.googlemerchandisestore.com/Google+Redesign/Apparel/Android+Embroidered+Crewneck+Sweater",
            PriceInfo = priceInfo2,
            ColorInfo = colorInfo2,
            RetrievableFields = fieldMask2,
        };

        product2.Categories.Add("Apparel");
        product2.Brands.Add("Android");
        product2.FulfillmentInfo.Add(fulfillmentInfo2);

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
    private static ImportProductsRequest GetImportProductsInlineRequest(List<Product> productsToImport, string projectId)
    {
        var defaultCatalog = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch";

        // To check error handling paste the invalid catalog name here:
        // defaultCatalog = "invalid_catalog_name";

        var inlineSource = new ProductInlineSource();
        inlineSource.Products.AddRange(productsToImport);

        var importRequest = new ImportProductsRequest
        {
            Parent = defaultCatalog,
            InputConfig = new ProductInputConfig
            {
                ProductInlineSource = inlineSource
            }
        };

        Console.WriteLine("Import products from inline source. request:");
        Console.WriteLine(importRequest);
        Console.WriteLine();

        return importRequest;
    }

    /// <summary>
    /// Call the Retail API to import products.
    /// </summary>
    public Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromInlineSource(string projectId)
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
// [END retail_import_products_from_inline_source]

/// <summary>
/// The import products inline source tutorial class.
/// </summary>
public static class ImportProductsInlineSourceTutorial
{
    [Runner.Attributes.Example]
    public static Operation<ImportProductsResponse, ImportMetadata> ImportProductsFromInlineSource()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new ImportProductsInlineSourceSample();
        return sample.ImportProductsFromInlineSource(projectId);
    }
}