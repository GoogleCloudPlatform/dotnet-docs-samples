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

// Delete product from a catalog using Retail API

using Google.Cloud.Retail.V2;
using System;

/// <summary>
/// The delete product sample class.
/// </summary>
public class DeleteProductSample
{
    /// <summary>
    /// Get delete product request.
    /// </summary>
    /// <param name="productName">The product name to delete.</param>
    /// <returns>Delete product request.</returns>
    private static DeleteProductRequest GetDeleteProductRequest(string productName)
    {
        DeleteProductRequest deleteProductRequest = new DeleteProductRequest
        {
            Name = productName
        };

        Console.WriteLine("Delete product request:");
        Console.WriteLine($"Product Name: {deleteProductRequest.Name}");
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
        DeleteProductRequest deleteProductRequest = GetDeleteProductRequest(productName);

        ProductServiceClient client = ProductServiceClient.Create();

        Console.WriteLine($"Deleting product:");

        client.DeleteProduct(deleteProductRequest);

        Console.WriteLine($"Product {productName} was deleted");
        Console.WriteLine();
    }
}

/// <summary>
/// The delete product tutorial class.
/// </summary>
public static class DeleteProductTutorial
{
    [Runner.Attributes.Example]
    public static void PerformDeleteProductOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Delete created product.
        DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
    }
}
