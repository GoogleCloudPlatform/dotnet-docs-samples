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

// [START retail_set_inventory]

using Google.Cloud.Retail.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading;

/// <summary>
/// The set inventory sample class.
/// </summary>
public class SetInventorySample
{
    /// <summary>
    /// Get the retail product with inventory info.
    /// </summary>
    /// <param name="product">The actual product.</param>
    /// <returns>Retail product with updated inventory info.</returns>
    private static Product SetInventoryForProduct(Product product)
    {
        string[] placeIds = { "store1", "store2" };

        product.FulfillmentInfo[0].Type = "pickup-in-store";
        product.FulfillmentInfo[0].PlaceIds.Clear();
        product.FulfillmentInfo[0].PlaceIds.AddRange(placeIds);
        product.PriceInfo.Price = 15.0f;
        product.PriceInfo.OriginalPrice = 20.0f;
        product.PriceInfo.Cost = 8.0f;
        product.PriceInfo.CurrencyCode = "USD";

        return product;
    }

    /// <summary>
    /// Get the set inventory request.
    /// </summary>
    /// <param name="product">The actual product.</param>
    /// <returns>Set inventory request.</returns>
    private static SetInventoryRequest GetSetInventoryRequest(Product product)
    {
        SetInventoryRequest setInventoryRequest = new SetInventoryRequest
        {
            Inventory = SetInventoryForProduct(product),
            AllowMissing = true,
            SetMask = new FieldMask
            {
                Paths = { "price_info", "availability", "fulfillment_info", "available_quantity" }
            }
        };

        Console.WriteLine("Set inventory. request:");
        Console.WriteLine($"Product name: {setInventoryRequest.Inventory.Name }");
        Console.WriteLine($"Product fultillment info: {setInventoryRequest.Inventory.FulfillmentInfo}");
        Console.WriteLine();

        return setInventoryRequest;
    }

    /// <summary>
    /// Call the Retail API to set product inventory
    /// </summary>
    /// <param name="product">The actual product.</param>
    private static void SetProductInventory(Product product)
    {
        SetInventoryRequest setInventoryRequest = GetSetInventoryRequest(product);

        ProductServiceClient client = ProductServiceClient.Create();
        client.SetInventory(setInventoryRequest);

        // This is a long running operation and its result is not immediately present with get operations,
        // thus we simulate wait with sleep method.
        Console.WriteLine("Set inventory. Wait 50 seconds:");
        Console.WriteLine();

        Thread.Sleep(50000);
    }

    /// <summary>
    /// Perform inventory setting
    /// </summary>
    /// <param name="product">The actual product.</param>
    public void PerformSetInventoryOperation(Product product)
    {
        SetProductInventory(product);
    }
}
// [END retail_set_inventory]

/// <summary>
/// THe set inventory tutorial class.
/// </summary>
public static class SetInventoryTutorial
{
    [Runner.Attributes.Example]
    public static void PerformSetInventoryOperation()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        var sample = new SetInventorySample();

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Set inventory for product.
        sample.PerformSetInventoryOperation(createdProduct);

        // Get product.
        Product inventoryProduct = GetProductSample.GetRetailProduct(createdProduct.Name);

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(inventoryProduct.Name);
    }
}