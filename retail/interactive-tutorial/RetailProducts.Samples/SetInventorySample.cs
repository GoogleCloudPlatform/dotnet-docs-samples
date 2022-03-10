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
using Google.LongRunning;
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
    /// <param name="product">The actual product.</param>
    /// <returns>Retail product with updated inventory info.</returns>
    private static Product SetInventoryForProduct(Product product)
    {
        FulfillmentInfo newFulfillmentInfo = new FulfillmentInfo
        {
            Type = "pickup-in-store",
            PlaceIds = { "store1", "store2" }
        };

        product.FulfillmentInfo.Clear();
        product.FulfillmentInfo.Add(newFulfillmentInfo);
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
    private static SetInventoryRequest GetSetInventoryRequest(Product productWithInventory)
    {
        // The request timestamp.
        DateTime requestTimeStamp = DateTime.Now.ToUniversalTime();

        // The out-of-order request timestamp:
        // requestTimeStamp = DateTime.Now.ToUniversalTime().AddDays(-1);

        SetInventoryRequest setInventoryRequest = new SetInventoryRequest
        {
            Inventory = productWithInventory,
            SetTime = Timestamp.FromDateTime(requestTimeStamp),
            AllowMissing = true,
            SetMask = new FieldMask
            {
                Paths = { "price_info", "availability", "fulfillment_info", "available_quantity" }
            }
        };

        Console.WriteLine("Set inventory request:");
        Console.WriteLine($"Product name: {setInventoryRequest.Inventory.Name }");
        Console.WriteLine($"Product fultillment info: {setInventoryRequest.Inventory.FulfillmentInfo}");
        Console.WriteLine();

        return setInventoryRequest;
    }

    /// <summary>
    /// Call the Retail API to set product inventory
    /// </summary>
    /// <param name="product">The actual product.</param>
    public static void SetProductInventory(Product product)
    {
        Product productWithInventory = SetInventoryForProduct(product);
        SetInventoryRequest setInventoryRequest = GetSetInventoryRequest(productWithInventory);

        ProductServiceClient client = ProductServiceClient.Create();

        // Make the request.
        Operation<SetInventoryResponse, SetInventoryMetadata> response = client.SetInventory(setInventoryRequest);

        Console.WriteLine("The operation was started:");
        Console.WriteLine(response.Name);
        Console.WriteLine();

        Console.WriteLine("Please wait till opeartion is done");
        Console.WriteLine();

        // Poll until the returned long-running operation is complete.
        Operation<SetInventoryResponse, SetInventoryMetadata> setInventoryResult = response.PollUntilCompleted();

        Console.WriteLine("Set inventory operation is done");
        Console.WriteLine();

        Console.WriteLine("Operation result:");
        Console.WriteLine(setInventoryResult.Result);
        Console.WriteLine();
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

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Set inventory for product.
        SetInventorySample.SetProductInventory(createdProduct);

        // Get product.
        Product inventoryProduct = GetProductSample.GetRetailProduct(createdProduct.Name);

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(inventoryProduct.Name);
    }
}