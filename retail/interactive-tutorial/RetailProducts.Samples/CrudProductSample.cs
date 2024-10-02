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

using Google.Cloud.Retail.V2;
using System;

/// <summary>
/// The crud product sample class.
/// </summary>
public class CrudProductSample
{
    /// <summary>
    /// Generate the product.
    /// </summary>
    /// <returns>Generated product.</returns>
    private static Product GenerateProduct()
    {
        return new Product
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
    }

    /// <summary>
    /// Get the product for update.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    /// <returns>Generated product for update.</returns>
    private static Product GenerateProductForUpdate(string productName)
    {
        return new Product
        {
            Name = productName,
            Title = "Updated Nest Mini",
            Type = Product.Types.Type.Primary,
            PriceInfo = new PriceInfo
            {
                Price = 20.0f,
                OriginalPrice = 25.5f,
                CurrencyCode = "EUR"
            },
            Availability = Product.Types.Availability.OutOfStock,
            Categories = { "Updated Speakers and displays" },
            Brands = { "Updated Google" }
        };
    }

    /// <summary>
    /// Call the Retail API to create a product.
    /// </summary>
    /// <param name="defaultBranchName">The default branch name.</param>
    /// <returns>Created retail product.</returns>
    public static Product CreateRetailProduct(string defaultBranchName)
    {
        Product generatedProduct = GenerateProduct();
        CreateProductRequest createProductRequest = new CreateProductRequest
        {
            Product = generatedProduct,
            ProductId = generatedProduct.Id,
            Parent = defaultBranchName
        };

        Console.WriteLine("Create product request:");
        Console.WriteLine($"Product: {createProductRequest.Product}");
        Console.WriteLine($"ProductId: {createProductRequest.ProductId}");
        Console.WriteLine($"Parent: {createProductRequest.Parent}");
        Console.WriteLine();

        ProductServiceClient client = ProductServiceClient.Create();
        Product createdProduct = client.CreateProduct(createProductRequest);

        Console.WriteLine("Created product:");
        Console.WriteLine($"Product title: {createdProduct.Title}");
        Console.WriteLine($"ProductId: {createProductRequest.ProductId}");
        Console.WriteLine($"Parent: {createProductRequest.Parent}");
        Console.WriteLine();

        return createdProduct;
    }

    /// <summary>
    /// Call the Retail API to get a product
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    /// <returns>Retail product.</returns>
    public static Product GetRetailProduct(string productName)
    {
        GetProductRequest getProductRequest = new GetProductRequest
        {
            Name = productName
        };

        Console.WriteLine("Get product request:");
        Console.WriteLine($"Product name: {getProductRequest.Name}");
        Console.WriteLine();

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

    /// <summary>
    /// Call the Retail API to update a product.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    /// <returns>Updated retail product.</returns>
    public static Product UpdateRetailProduct(string productName)
    {
        Product generatedProductForUpdate = GenerateProductForUpdate(productName);

        UpdateProductRequest updateProductRequest = new UpdateProductRequest
        {
            Product = generatedProductForUpdate,
            AllowMissing = true
        };

        Console.WriteLine("Update product request:");
        Console.WriteLine($"Product Name: {updateProductRequest.Product.Name}");
        Console.WriteLine($"Product title: {updateProductRequest.Product.Title}");
        Console.WriteLine();

        ProductServiceClient client = ProductServiceClient.Create();
        Product updatedProduct = client.UpdateProduct(updateProductRequest);

        Console.WriteLine($"Update title: {updatedProduct.Title}");
        Console.WriteLine();
        return updatedProduct;
    }

    /// <summary>
    /// Call the Retail API to delete a product
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    public static void DeleteRetailProduct(string productName)
    {
        DeleteProductRequest deleteProductRequest = new DeleteProductRequest
        {
            Name = productName
        };

        Console.WriteLine("Delete product request:");
        Console.WriteLine($"Product Name: {deleteProductRequest.Name}");
        Console.WriteLine();

        ProductServiceClient client = ProductServiceClient.Create();
        client.DeleteProduct(deleteProductRequest);

        Console.WriteLine($"Deleting product:");
        Console.WriteLine($"Product {productName} was deleted");
        Console.WriteLine();
    }
}

/// <summary>
/// The crud product tutorial class.
/// </summary>
public static class CrudProductTutorial
{
    [Runner.Attributes.Example]
    public static void PerformCRUDProductOperations()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        string defaultBranchName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch";

        Product createdProduct = CrudProductSample.CreateRetailProduct(defaultBranchName);
        CrudProductSample.GetRetailProduct(createdProduct.Name);
        CrudProductSample.UpdateRetailProduct(createdProduct.Name);
        CrudProductSample.DeleteRetailProduct(createdProduct.Name);
    }
}
