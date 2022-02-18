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

// [START retail_update_product]
// Update product from a catalog using Retail API

using Google.Cloud.Retail.V2;
using System;
using System.Linq;

/// <summary>
/// The update product sample class.
/// </summary>
public class UpdateProductSample
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
    /// Generate product for update.
    /// </summary>
    /// <param name="productId">The actual product id.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Generated retail product.</returns>
    private static Product GenerateProductForUpdate(string productId, string projectId)
    {
        var updatedPriceInfo = new PriceInfo
        {
            Price = 20.0f,
            OriginalPrice = 25.5f,
            CurrencyCode = "EUR"
        };

        var generatedProduct = new Product
        {
            Id = productId,
            Name = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch/products/{productId}",
            Title = "Updated Nest Mini",
            Type = Product.Types.Type.Primary,
            PriceInfo = updatedPriceInfo,
            Availability = Product.Types.Availability.OutOfStock
        };

        generatedProduct.Categories.Add("Updated Speakers and displays");
        generatedProduct.Brands.Add("Updated Google");

        return generatedProduct;
    }

    /// <summary>
    /// Get the update product request.
    /// </summary>
    /// <param name="productToUpdate">The product to update object.</param>
    /// <returns>Update product request.</returns>
    private static UpdateProductRequest GetUpdateProductRequest(Product productToUpdate)
    {
        var updateProductRequest = new UpdateProductRequest
        {
            Product = productToUpdate,
            AllowMissing = true
        };

        Console.WriteLine("Update product. request:");
        Console.WriteLine(updateProductRequest);
        Console.WriteLine();

        return updateProductRequest;
    }

    /// <summary>
    /// Call the Retail API to update a product.
    /// </summary>
    /// <param name="originalProduct">The original product object.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Updated retail product.</returns>
    public static Product UpdateRetailProduct(Product originalProduct, string projectId)
    {
        var productForUpdate = GenerateProductForUpdate(originalProduct.Id, projectId);
        var updateProductRequest = GetUpdateProductRequest(productForUpdate);
        ProductServiceClient client = ProductServiceClient.Create();
        Product updatedProduct = client.UpdateProduct(updateProductRequest);

        Console.WriteLine("Updated product:");
        Console.WriteLine(updatedProduct);
        Console.WriteLine();

        return updatedProduct;
    }

    /// <summary>
    /// Perform the update product operations.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Updated retail product.</returns>
    public Product PerformUpdateProductOperation(string projectId)
    {
        // Create product.
        var originalProduct = CreateProductSample.CreateRetailProduct(GeneratedProductId, projectId);

        // Update created product.
        var updatedProduct = UpdateRetailProduct(originalProduct, projectId);

        // Delete updated product.
        DeleteProductSample.DeleteRetailProduct(updatedProduct.Name);

        return updatedProduct;
    }
}
// [END retail_update_product]

/// <summary>
/// The update product tutorial class.
/// </summary>
public static class UpdateProductTutorial
{
    [Runner.Attributes.Example]
    public static Product PerformUpdateProductOperation()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new UpdateProductSample();
        return sample.PerformUpdateProductOperation(projectId);
    }
}