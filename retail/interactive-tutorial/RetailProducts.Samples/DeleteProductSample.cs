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

// [START retail_delete_product]
// Delete product from a catalog using Retail API

using Google.Cloud.Retail.V2;
using System;
using System.Linq;

/// <summary>
/// The delete product sample class.
/// </summary>
public class DeleteProductSample
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
    /// Get delete product request.
    /// </summary>
    /// <param name="productName">The product name to delete.</param>
    /// <returns>Delete product request.</returns>
    private static DeleteProductRequest GetDeleteProductRequest(string productName)
    {
        var deleteProductRequest = new DeleteProductRequest
        {
            Name = productName
        };

        Console.WriteLine("Delete product. request:");
        Console.WriteLine(deleteProductRequest);
        Console.WriteLine();

        return deleteProductRequest;
    }

    /// <summary>
    ///  Call the Retail API to delete a product.
    /// </summary>
    /// <param name="productName">The current product name.</param>
    /// <returns>Created product.</returns>
    public static void DeleteRetailProduct(string productName)
    {
        var deleteProductRequest = GetDeleteProductRequest(productName);

        var client = ProductServiceClient.Create();
        client.DeleteProduct(deleteProductRequest);

        Console.WriteLine($"Deleting product:");
        Console.WriteLine($"Product {productName} was deleted");
        Console.WriteLine();
    }

    /// <summary>
    /// Perform product deletion.
    /// </summary>
    /// <param name="projectId">The current project id.</param>
    /// <returns>Created product.</returns>
    public void DeleteProduct(string projectId)
    {
        // Create product
        var createdProduct = CreateProductSample.CreateRetailProduct(GeneratedProductId, projectId);

        // Delete created product
        DeleteRetailProduct(createdProduct.Name);
    }
}
// [END retail_delete_product]

/// <summary>
/// The delete product tutorial class.
/// </summary>
public static class DeleteProductTutorial
{
    [Runner.Attributes.Example]
    public static void PerformDeleteProductOperation()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new DeleteProductSample();
        sample.DeleteProduct(projectId);
    }
}