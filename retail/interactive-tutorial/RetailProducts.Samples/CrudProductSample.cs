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

// [START retail_crud_product]
// Create product in a catalog using Retail API

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

/// <summary>
/// The crud product sample class.
/// </summary>
public class CrudProductSample
{
    private const string ProductId = "crud_product_id";

    private static readonly string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    private static readonly string defaultBranchName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch";
    private static readonly string productName = $"{defaultBranchName}/products/{ProductId}";

    /// <summary>
    /// Generate the product.
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
    /// Get the product for update.
    /// </summary>
    /// <returns></returns>
    private static Product GenerateProductForUpdate()
    {
        var updatedPriceInfo = new PriceInfo
        {
            Price = 20.0f,
            OriginalPrice = 25.5f,
            CurrencyCode = "EUR"
        };

        string[] categories = { "Updated Speakers and displays" };
        string[] brands = { "Updated Google" };

        var generatedProduct = new Product
        {
            Id = ProductId,
            Name = productName,
            Title = "Updated Nest Mini",
            Type = Product.Types.Type.Primary,
            PriceInfo = updatedPriceInfo,
            Availability = Product.Types.Availability.OutOfStock
        };

        generatedProduct.Categories.Add(categories);
        generatedProduct.Brands.Add(brands);

        return generatedProduct;
    }

    /// <summary>
    /// Call the Retail API to create a product.
    /// </summary>
    /// <returns>Created retail product.</returns>
    private static Product CreateRetailProduct()
    {
        var generatedProduct = GenerateProduct();
        var createProductRequest = new CreateProductRequest
        {
            Product = generatedProduct,
            ProductId = ProductId,
            Parent = defaultBranchName
        };

        Console.WriteLine("Create product. request:");
        Console.WriteLine(createProductRequest);
        Console.WriteLine();

        ProductServiceClient client = ProductServiceClient.Create();
        Product createdProduct = client.CreateProduct(createProductRequest);

        Console.WriteLine("Created product:");
        Console.WriteLine(createdProduct);
        Console.WriteLine();

        return createdProduct;
    }

    /// <summary>
    /// Call the Retail API to get a product
    /// </summary>
    /// <returns>Retail product.</returns>
    private static Product GetRetailProduct()
    {
        var getProductRequest = new GetProductRequest
        {
            Name = productName
        };

        Console.WriteLine("Get product. request:");
        Console.WriteLine(getProductRequest);
        Console.WriteLine();

        ProductServiceClient client = ProductServiceClient.Create();
        Product product = client.GetProduct(getProductRequest);

        Console.WriteLine("Get product. response:");
        Console.WriteLine(product);
        Console.WriteLine();

        return product;
    }

    /// <summary>
    /// Call the Retail API to update a product.
    /// </summary>
    /// <returns>Updated retail product.</returns>
    private static Product UpdateRetailProduct()
    {
        var generatedProductForUpdate = GenerateProductForUpdate();

        var updateProductRequest = new UpdateProductRequest
        {
            Product = generatedProductForUpdate,
            AllowMissing = true
        };

        Console.WriteLine("Update product. request:");
        Console.WriteLine(updateProductRequest);
        Console.WriteLine();

        ProductServiceClient client = ProductServiceClient.Create();
        Product updatedProduct = client.UpdateProduct(updateProductRequest);

        Console.WriteLine("Updated product:");
        Console.WriteLine(updatedProduct);
        Console.WriteLine();
        return updatedProduct;
    }

    /// <summary>
    /// Call the Retail API to delete a product
    /// </summary>
    private static void DeleteRetailProduct()
    {
        var deleteProductRequest = new DeleteProductRequest
        {
            Name = productName
        };

        Console.WriteLine("Delete product. request:");
        Console.WriteLine(deleteProductRequest);
        Console.WriteLine();

        ProductServiceClient client = ProductServiceClient.Create();
        client.DeleteProduct(deleteProductRequest);

        Console.WriteLine($"Deleting product:");
        Console.WriteLine($"Product {productName} was deleted");
        Console.WriteLine();
    }

    /// <summary>
    /// Call the Retail API to delete a product if it exists.
    /// </summary>
    /// <param name="productName">The name of the product to delete.</param>
    private static void TryToDeleteRetailProductIfExists(string productName)
    {
        var getProductRequest = new GetProductRequest
        {
            Name = productName
        };

        var deleteProductRequest = new DeleteProductRequest
        {
            Name = productName
        };

        try
        {
            Console.WriteLine("Delete product from the catalog, if the product already exists");
            Console.WriteLine();

            ProductServiceClient client = ProductServiceClient.Create();
            Product product = client.GetProduct(getProductRequest);
            client.DeleteProduct(deleteProductRequest);
        }
        catch (Exception)
        {
            Console.WriteLine($"Product with name {productName} does not exist.");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Perform CRUD Product Operations.
    /// </summary>
    public void PerformCRUDProductOperations()
    {
        // Call the methods
        TryToDeleteRetailProductIfExists(productName);
        CreateRetailProduct();
        GetRetailProduct();
        UpdateRetailProduct();
        DeleteRetailProduct();
    }
}
// [END retail_crud_product]

/// <summary>
/// The crud product tutorial class.
/// </summary>
public static class CrudProductTutorial
{
    [Runner.Attributes.Example]
    public static void PerformCRUDProductOperations()
    {
        var sample = new CrudProductSample();
        sample.PerformCRUDProductOperations();
    }
}
