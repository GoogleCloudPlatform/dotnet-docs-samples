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

/// <summary>
/// The update product sample class.
/// </summary>
public class UpdateProductSample
{
    /// <summary>
    /// Generate product for update.
    /// </summary>
    /// <param name="originalProduct">The existing product to update.</param>
    /// <returns>Generated retail product.</returns>
    private static Product ModifyOriginalProduct(Product originalProduct)
    {
        originalProduct.Title = "Updated Nest Mini";
        originalProduct.Categories[0] = "Updated Speakers and displays";
        originalProduct.Brands[0] = "Updated Google";
        originalProduct.Availability = Product.Types.Availability.OutOfStock;
        originalProduct.PriceInfo.Price = 20.0f;
        originalProduct.PriceInfo.OriginalPrice = 25.5f;
        originalProduct.PriceInfo.CurrencyCode = "EUR";

        return originalProduct;
    }

    /// <summary>
    /// Get the update product request.
    /// </summary>
    /// <param name="productToUpdate">The product to update object.</param>
    /// <returns>Update product request.</returns>
    private static UpdateProductRequest GetUpdateProductRequest(Product productToUpdate)
    {
        UpdateProductRequest updateProductRequest = new UpdateProductRequest
        {
            Product = productToUpdate,
            AllowMissing = true
        };

        Console.WriteLine("Update product request:");
        Console.WriteLine($"Product Name: {productToUpdate.Name}");
        Console.WriteLine($"Product Title: {productToUpdate.Title}");
        Console.WriteLine($"Product Categories: {productToUpdate.Categories}");
        Console.WriteLine($"Product Brands: {productToUpdate.Brands}");
        Console.WriteLine();

        return updateProductRequest;
    }

    /// <summary>
    /// Call the Retail API to update a product.
    /// </summary>
    /// <param name="originalProduct">The original product object.</param>
    /// <returns>Updated retail product.</returns>
    public static Product UpdateRetailProduct(Product originalProduct)
    {
        Product modifiedProduct = ModifyOriginalProduct(originalProduct);
        UpdateProductRequest updateProductRequest = GetUpdateProductRequest(modifiedProduct);
        ProductServiceClient client = ProductServiceClient.Create();
        Product updatedProduct = client.UpdateProduct(updateProductRequest);

        Console.WriteLine($"Updated title: {updatedProduct.Title}");
        Console.WriteLine();

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
    public static void PerformUpdateProductOperation()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        // Create product.
        Product originalProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Update product.
        Product updatedProduct = UpdateProductSample.UpdateRetailProduct(originalProduct);

        // Delete updated product.
        DeleteProductSample.DeleteRetailProduct(updatedProduct.Name);
    }
}