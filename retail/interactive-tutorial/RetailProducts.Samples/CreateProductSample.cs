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

// Create product in a catalog using Retail API

using Google.Cloud.Retail.V2;
using System;

/// <summary>
/// The create product sample class.
/// </summary>
public class CreateProductSample
{
    /// <summary>
    /// Generate product with fulfillment info.
    /// </summary>
    /// <returns>Generated product.</returns>
    private static Product GenerateProductWithFulfillment()
    {
        Product generatedProduct = new Product
        {
            Id = Guid.NewGuid().ToString("N").Substring(0, 14),
            Title = "Nest Mini",
            Type = Product.Types.Type.Primary,
            PriceInfo = new PriceInfo
            {
                Price = 30.0f,
                OriginalPrice = 35.5f,
                CurrencyCode = "USD"
            },
            Availability = Product.Types.Availability.InStock,
            Categories = { "Speakers and displays" },
            Brands = { "Google" }
        };

        string[] placeIds = { "store0", "store1" };

        FulfillmentInfo fulfillmentInfo = new FulfillmentInfo
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
    /// <param name="projectId">The current project id.</param>
    /// <returns>Create product request.</returns>
    private static CreateProductRequest GetCreateProductRequest(Product productToCreate, string projectId)
    {
        string defaultBranchName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch";

        CreateProductRequest createProductRequest = new CreateProductRequest
        {
            Product = productToCreate,
            ProductId = productToCreate.Id,
            Parent = defaultBranchName
        };

        Console.WriteLine("Create product request:");
        Console.WriteLine($"Product: {createProductRequest.Product}");
        Console.WriteLine($"ProductId: {createProductRequest.ProductId}");
        Console.WriteLine($"Parent: {createProductRequest.Parent}");
        Console.WriteLine();

        return createProductRequest;
    }

    /// <summary>
    /// Call the Retail API to create a product.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Created product.</returns>
    public static Product CreateRetailProductWithFulfillment(string projectId)
    {
        Product generatedProduct = GenerateProductWithFulfillment();
        CreateProductRequest createProductRequest = GetCreateProductRequest(generatedProduct, projectId);

        ProductServiceClient client = ProductServiceClient.Create();
        Product createdProduct = client.CreateProduct(createProductRequest);

        Console.WriteLine("Created product:");
        Console.WriteLine($"Product title: {createdProduct.Title}");
        Console.WriteLine($"Product name: {createdProduct.Name}");
        Console.WriteLine($"Product id: {createProductRequest.ProductId}");
        Console.WriteLine($"Product fulfillment info: {createdProduct.FulfillmentInfo}");
        Console.WriteLine();

        return createdProduct;
    }
}

/// <summary>
/// The create product tutorial class.
/// </summary>
public static class CreateProductTutorial
{
    [Runner.Attributes.Example]
    public static void PerformCreateProductOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Delete created product.
        DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
    }
}
