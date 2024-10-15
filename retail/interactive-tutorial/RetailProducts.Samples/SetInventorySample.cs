// Copyright 2022 Google Inc.

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
using Google.Protobuf.WellKnownTypes;
using System;

/// <summary>
/// The set inventory sample class.
/// </summary>
public class SetInventorySample
{
    /// <summary>
    /// Get the retail product with inventory info.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    /// <returns>Retail product with updated inventory info.</returns>
    private static Product GetProductWithInventoryInfo(string productName)
    {
        var priceInfo = new PriceInfo
        {
            Price = 15.0f,
            OriginalPrice = 20.0f,
            Cost = 8.0f,
            CurrencyCode = "USD"
        };

        var fulfillmentInfo = new FulfillmentInfo
        {
            Type = "pickup-in-store",
            PlaceIds = { "store1", "store2" }
        };

        var product = new Product
        {
            Name = productName,
            PriceInfo = priceInfo,
            Availability = Product.Types.Availability.InStock,
            FulfillmentInfo = { fulfillmentInfo }
        };

        return product;
    }

    /// <summary>
    /// Get the set inventory request.
    /// </summary>
    /// <param name="productName">The actual product with inventory info.</param>
    /// <returns>Set inventory request.</returns>
    private static SetInventoryRequest GetSetInventoryRequest(string productName)
    {
        var setMask = new FieldMask
        {
            Paths = { "price_info", "availability", "fulfillment_info", "available_quantity" }
        };

        var setInventoryRequest = new SetInventoryRequest
        {
            Inventory = GetProductWithInventoryInfo(productName),
            AllowMissing = true,
            SetMask = setMask
        };

        // To send an out-of-order request assign the invalid SetTime here:
        // setInventoryRequest.SetTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-1));

        Console.WriteLine("Set inventory request:");
        Console.WriteLine($"Product Name: {setInventoryRequest.Inventory.Name}");
        Console.WriteLine($"Product Title: {setInventoryRequest.Inventory.Title}");
        Console.WriteLine($"Product Brands: {setInventoryRequest.Inventory.Brands}");
        Console.WriteLine($"Product Categories: {setInventoryRequest.Inventory.Categories}");
        Console.WriteLine($"Product Fulfillment Info: {setInventoryRequest.Inventory.FulfillmentInfo}");
        Console.WriteLine($"Product Price Info: {setInventoryRequest.Inventory.PriceInfo}");
        Console.WriteLine();

        return setInventoryRequest;
    }

    /// <summary>
    /// Call the Retail API to set product inventory.
    /// </summary>
    /// <param name="product">The actual product.</param>
    public static void SetProductInventory(string productName)
    {
        var setInventoryRequest = GetSetInventoryRequest(productName);
        var client = ProductServiceClient.Create();
        var operation = client.SetInventory(setInventoryRequest);

        Console.WriteLine("Set inventory. Please, wait.");
        Console.WriteLine();

        operation.PollUntilCompleted();
    }
}

/// <summary>
/// The set inventory tutorial class.
/// </summary>
public static class SetInventoryTutorial
{
    [Runner.Attributes.Example]
    public static void PerformSetInventoryOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Set inventory for product.
        SetInventorySample.SetProductInventory(createdProduct.Name);

        // Get product.
        Product product = GetProductSample.GetRetailProduct(createdProduct.Name);

        Console.WriteLine($"Product Price Info: {product.PriceInfo}");
        Console.WriteLine();

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
    }
}
