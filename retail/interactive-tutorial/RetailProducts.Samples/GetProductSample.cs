// Copyright 2021 Google Inc.

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

// Get product from a catalog using Retail API

using Google.Cloud.Retail.V2;
using System;

/// <summary>
/// The get product sample class.
/// </summary>
public class GetProductSample
{
    /// <summary>
    /// Get the create product request.
    /// </summary>
    /// <param name="productName">The name of the product.</param>
    /// <returns>Get product request.</returns>
    private static GetProductRequest GetProductRequest(string productName)
    {
        GetProductRequest getProductRequest = new GetProductRequest
        {
            Name = productName
        };

        Console.WriteLine("Get product request:");
        Console.WriteLine($"Product name: {getProductRequest.Name}");
        Console.WriteLine();

        return getProductRequest;
    }

    /// <summary>
    /// Call the Retail API to get a product
    /// </summary>
    /// <param name="productName">The name of the product.</param>
    /// <returns>Retrieved product.</returns>
    public static Product GetRetailProduct(string productName)
    {
        GetProductRequest getProductRequest = GetProductRequest(productName);

        ProductServiceClient client = ProductServiceClient.Create();
        Product product = client.GetProduct(getProductRequest);

        Console.WriteLine("Get product response:");
        Console.WriteLine($"Product Name: {product.Name}");
        Console.WriteLine($"Product Title: {product.Title}");
        Console.WriteLine($"Product Brands: {product.Brands}");
        Console.WriteLine($"Product Categories: {product.Categories}");
        Console.WriteLine($"Product Fulfillment Info: {product.FulfillmentInfo}");
        Console.WriteLine();

        return product;
    }
}

/// <summary>
/// The get product tutorial class.
/// </summary>
public static class GetProductTutorial
{
    [Runner.Attributes.Example]
    public static void PerformGetProductOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Get created product.
        Product retrievedProduct = GetProductSample.GetRetailProduct(createdProduct.Name);

        // Delete created product.
        DeleteProductSample.DeleteRetailProduct(retrievedProduct.Name);
    }
}
