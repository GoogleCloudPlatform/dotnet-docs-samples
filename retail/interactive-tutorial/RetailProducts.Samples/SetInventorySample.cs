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

// [START set_inventory]

using Google.Cloud.Retail.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading;

/// <summary>
/// The set inventory sample class.
/// </summary>
public class SetInventorySample
{
    private const string ProductId = "inventory_test_product_id";

    /// <summary>
    /// Get the retail product with inventory info.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    /// <returns>Generated retail product with inventory info.</returns>
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
            Type = "pickup-in-store"
        };

        string[] placeIds = { "store1", "store2" };

        fulfillmentInfo.PlaceIds.Add(placeIds);

        var product = new Product
        {
            Name = productName,
            PriceInfo = priceInfo,
            Availability = Product.Types.Availability.InStock
        };

        product.FulfillmentInfo.Add(fulfillmentInfo);

        return product;
    }

    /// <summary>
    /// Get the set inventory request.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    /// <returns>Set inventory request.</returns>
    private static SetInventoryRequest GetSetInventoryRequest(string productName)
    {
        // The request timestamp.
        DateTime requestTimeStamp = DateTime.Now.ToUniversalTime();

        // The out-of-order request timestamp:
        // requestTimeStamp = DateTime.Now.ToUniversalTime().AddDays(-1);

        string[] paths = { "price_info", "availability", "fulfillment_info", "available_quantity" };
        var setMask = new FieldMask();
        setMask.Paths.AddRange(paths);

        var setInventoryRequest = new SetInventoryRequest
        {
            Inventory = GetProductWithInventoryInfo(productName),
            SetTime = Timestamp.FromDateTime(requestTimeStamp),
            AllowMissing = true,
            SetMask = setMask
        };

        Console.WriteLine("Set inventory. request:");
        Console.WriteLine(setInventoryRequest);
        Console.WriteLine();

        return setInventoryRequest;
    }

    /// <summary>
    /// Call the Retail API to set product inventory
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    private static void SetProductInventory(string productName)
    {
        var setInventoryRequest = GetSetInventoryRequest(productName);

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
    /// <param name="projectId">The current project id</param>
    /// <returns>Created and deleted retail product object.</returns>
    public Product PerformSetInventoryOperation(string projectId)
    {
        string productName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch/products/{ProductId}";

        CreateProductSample.CreateRetailProductWithFulfillment(ProductId, projectId);
        SetProductInventory(productName);
        var inventoryProduct = GetProductSample.GetRetailProduct(productName);
        DeleteProductSample.DeleteRetailProduct(productName);

        return inventoryProduct;
    }
}
// [END set_inventory]

/// <summary>
/// THe set inventory tutorial class.
/// </summary>
public static class SetInventoryTutorial
{
    [Runner.Attributes.Example]
    public static Product PerformSetInventoryOperation()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new SetInventorySample();
        return sample.PerformSetInventoryOperation(projectId);
    }
}