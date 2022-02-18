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

// [START retail_get_product]
// Get product from a catalog using Retail API

using Google.Cloud.Retail.V2;
using System;
using System.Linq;

/// <summary>
/// The get product sample class.
/// </summary>
public class GetProductSample
{
    private static readonly Random Random = new Random();
    private static readonly string GeneratedProductId = RandomAlphanumericString(14);

    /// <summary>
    /// Generate the random alphanumeric string.
    /// </summary>
    /// <param name="length">The required length of alphanumeric string.</param>
    /// <returns>Generated alphanumeric string.</returns>
    private static string RandomAlphanumericString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Get the create product request.
    /// </summary>
    /// <param name="productName">The name of the product.</param>
    /// <returns>Get product request.</returns>
    private static GetProductRequest GetProductRequest(string productName)
    {
        var getProductRequest = new GetProductRequest
        {
            Name = productName
        };

        Console.WriteLine("Get product. request:");
        Console.WriteLine(getProductRequest);
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
        var getProductRequest = GetProductRequest(productName);

        ProductServiceClient client = ProductServiceClient.Create();
        Product product = client.GetProduct(getProductRequest);

        Console.WriteLine("Get product. response:");
        Console.WriteLine(product);
        Console.WriteLine();

        return product;
    }

    /// <summary>
    /// Perform the product retrieval operation.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    public Product PerformGetProductOperation(string projectId)
    {
        // Create product.
        var createdProduct = CreateProductSample.CreateRetailProduct(GeneratedProductId, projectId);

        // Get created product.
        var retrievedProduct = GetRetailProduct(createdProduct.Name);

        // Delete created product.
        DeleteProductSample.DeleteRetailProduct(retrievedProduct.Name);

        return retrievedProduct;
    }
}
// [END retail_get_product]

/// <summary>
/// The get product tutorial class.
/// </summary>
public static class GetProductTutorial
{
    [Runner.Attributes.Example]
    public static Product PerformGetProductOperation()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new GetProductSample();
        return sample.PerformGetProductOperation(projectId);
    }
}