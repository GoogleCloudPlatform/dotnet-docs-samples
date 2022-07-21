// Copyright 2022 Google Inc. All Rights Reserved.
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
    /// <param name="productName">The actual product with inventory info.</param>
    /// <returns>Set inventory request.</returns>
    private static SetInventoryRequest GetSetInventoryRequest(string productName)
    {
        // The request timestamp
        DateTime requestTimeStamp = DateTime.Now.ToUniversalTime();

        // The out-of-order request timestamp
        // request_time = datetime.datetime.now() - datetime.timedelta(days=1)

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

        var jsonSerializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        var setInventoryRequestJson = JsonConvert.SerializeObject(setInventoryRequest, jsonSerializeSettings);

        Console.WriteLine("\nSet inventory request: \n\n" + setInventoryRequestJson);
        return setInventoryRequest;
    }

    /// <summary>
    /// Call the Retail API to set product inventory.
    /// </summary>
    /// <param name="product">The actual product.</param>
    public static void SetProductInventory(string productName)
    {
        var setInventoryRequest = GetSetInventoryRequest(productName);
        GetProductServiceClient().SetInventory(setInventoryRequest);

        // This is a long running operation and its result is not immediately present with get operations,
        // thus we simulate wait with sleep method.
        Console.WriteLine("\nSet inventory. Wait 50 seconds:\n");
        Thread.Sleep(50000);
    }

    /// <summary>
    /// Get product service client.
    /// </summary>
    private static ProductServiceClient GetProductServiceClient()
    {
        string Endpoint = "retail.googleapis.com";

        var productServiceClientBuilder = new ProductServiceClientBuilder
        {
            Endpoint = Endpoint
        };

        var productServiceClient = productServiceClientBuilder.Build();
        return productServiceClient;
    }
}
// [END retail_set_inventory]

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

        Console.WriteLine($"Product price info: {product.PriceInfo}");
        Console.WriteLine();

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
    }
}