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

// [START retail_create_product]
// Create product in a catalog using Retail API

using Google.Cloud.Retail.V2;
using System;
using System.Linq;

/// <summary>
/// The create product sample class.
/// </summary>
public class CreateProductSample
{
    private static readonly Random Random = new Random();
    private static readonly string GeneratedProductId = RandomAlphanumericString(14);

    /// <summary>
    /// Generate the random alphanumeric string.
    /// </summary>
    /// <param name="length">The required length of alphanumeric string.</param>
    /// <returns>Generated alphanumeric string.</returns>
    public static string RandomAlphanumericString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Generate product.
    /// </summary>
    /// <returns>Generated product.</returns>
    private static Product GenerateProduct()
    {
        var priceInfo = new PriceInfo
        {
            Price = 30.0f,
            OriginalPrice = 35.5f,
            CurrencyCode = "USD"
        };

        string[] brands = { "Google" };
        string[] categories = { "Speakers and displays" };

        var generatedProduct = new Product
        {
            Title = "Nest Mini",
            Type = Product.Types.Type.Primary,
            PriceInfo = priceInfo,
            Availability = Product.Types.Availability.InStock
        };

        generatedProduct.Categories.Add(categories);
        generatedProduct.Brands.Add(brands);

        return generatedProduct;
    }

    /// <summary>
    /// Generate product with fulfillment info.
    /// </summary>
    /// <returns>Generated product.</returns>
    private static Product GenerateProductWithFulfillment()
    {
        var generatedProduct = GenerateProduct();

        string[] placeIds = { "store0", "store1" };

        var fulfillmentInfo = new FulfillmentInfo
        {
            Type = "pickup-in-store"
        };

        fulfillmentInfo.PlaceIds.AddRange(placeIds);

        generatedProduct.FulfillmentInfo.Add(fulfillmentInfo);

        return generatedProduct;
    }

    /// <summary>
    /// Get create product request.
    /// </summary>
    /// <param name="productToCreate">The actual product object to create.</param>
    /// <param name="productId">The product id.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Create product request.</returns>
    private static CreateProductRequest GetCreateProductRequest(Product productToCreate, string productId, string projectId)
    {
        string defaultBranchName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch";

        var createProductRequest = new CreateProductRequest
        {
            Product = productToCreate,
            ProductId = productId,
            Parent = defaultBranchName
        };

        Console.WriteLine("Create product. request:");
        Console.WriteLine($"Product: {createProductRequest.Product}");
        Console.WriteLine($"ProductId: {createProductRequest.ProductId}");
        Console.WriteLine($"Parent: {createProductRequest.Parent}");
        Console.WriteLine();

        return createProductRequest;
    }

    /// <summary>
    /// Call the Retail API to create a product.
    /// </summary>
    /// <param name="productId">The product id.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Created product.</returns>
    public static Product CreateRetailProduct(string productId, string projectId)
    {
        var generatedProduct = GenerateProduct();
        var createProductRequest = GetCreateProductRequest(generatedProduct, productId, projectId);

        var client = ProductServiceClient.Create();
        var createdProduct = client.CreateProduct(createProductRequest);

        Console.WriteLine("Created product:");
        Console.WriteLine(createdProduct);
        Console.WriteLine();

        return createdProduct;
    }

    /// <summary>
    /// Call the Retail API to create a product with fulfillment.
    /// </summary>
    /// <param name="productId">The product id.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Created product.</returns>
    public static Product CreateRetailProductWithFulfillment(string productId, string projectId)
    {
        var generatedProduct = GenerateProductWithFulfillment();
        var createProductRequest = GetCreateProductRequest(generatedProduct, productId, projectId);

        var client = ProductServiceClient.Create();
        var createdProduct = client.CreateProduct(createProductRequest);

        Console.WriteLine("Created product:");
        Console.WriteLine(createdProduct);
        Console.WriteLine();

        return createdProduct;
    }

    /// <summary>
    /// Perform product creation.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Created product.</returns>

    public Product CreateProduct(string projectId)
    {
        // Create product
        var createdProduct = CreateRetailProduct(GeneratedProductId, projectId);

        // Delete created product
        DeleteProductSample.DeleteRetailProduct(createdProduct.Name);

        return createdProduct; 
    }
}
// [END retail_create_product]

/// <summary>
/// The create product tutorial class.
/// </summary>
public static class CreateProductTutorial
{
    [Runner.Attributes.Example]
    public static Product PerformCreateProductOperation()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new CreateProductSample();
        return sample.CreateProduct(projectId);
    }
}