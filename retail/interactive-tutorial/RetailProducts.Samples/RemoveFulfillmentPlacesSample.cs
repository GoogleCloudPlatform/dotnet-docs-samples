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

// [START retail_remove_fulfillment_places]

using Google.Cloud.Retail.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading;

/// <summary>
/// The remove fulfillment places sample class.
/// </summary>
public class RemoveFulfillmentPlacesSample
{
    // The request timestamp
    private static readonly DateTime RequestTimeStamp = DateTime.Now.ToUniversalTime();

    // The outdated request timestamp:
    // RequestTimeStamp = DateTime.Now.ToUniversalTime().AddDays(-1);

    /// <summary>
    /// Get the remove fulfillment places request.
    /// </summary>
    /// <param name="productName">the actual name of the retail product.</param>
    /// <returns>The remove fulfillment places request.</returns>
    private static RemoveFulfillmentPlacesRequest GetRemoveFulfillmentRequest(string productName)
    {
        RemoveFulfillmentPlacesRequest removeFulfillmentRequest = new RemoveFulfillmentPlacesRequest
        {
            Product = productName,
            Type = "pickup-in-store",
            RemoveTime = Timestamp.FromDateTime(RequestTimeStamp),
            AllowMissing = true,
            PlaceIds = { "store0" }
        };

        Console.WriteLine("Remove fulfillment places. request:");
        Console.WriteLine($"Product Name: {removeFulfillmentRequest.Product}");
        Console.WriteLine($"Type: {removeFulfillmentRequest.Type}");
        Console.WriteLine($"Place Ids: {removeFulfillmentRequest.PlaceIds}");
        Console.WriteLine($"Remove Time: {removeFulfillmentRequest.RemoveTime}");
        Console.WriteLine();

        return removeFulfillmentRequest;
    }

    /// <summary>
    /// Call the Retail API to remove fulfillment places.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    private static void RemoveFulfillment(string productName)
    {
        RemoveFulfillmentPlacesRequest removeFulfillmentRequest = GetRemoveFulfillmentRequest(productName);

        ProductServiceClient client = ProductServiceClient.Create();
        client.RemoveFulfillmentPlaces(removeFulfillmentRequest);

        //This is a long running operation and its result is not immediately present with get operations,
        // thus we simulate wait with sleep method.
        Console.WriteLine("Remove fulfillment places. Wait 2 minutes:");
        Console.WriteLine();

        Thread.Sleep(120000);
    }

    /// <summary>
    /// Perform the remove fulfillment operations.
    /// </summary>
    /// <param name="productName">The name of the product.</param>
    /// <returns>Retail product with removed fulfillment places.</returns>
    public void PerformRemoveFulfillment(string productName)
    {
        RemoveFulfillment(productName);
    }
}
// [END retail_remove_fulfillment_places]

/// <summary>
/// The remove fulfillment places tutorial class.
/// </summary>
public static class RemoveFulfillmentPlacesTutorial
{
    [Runner.Attributes.Example]
    public static void PerformRemoveFulfillment()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        var sample = new RemoveFulfillmentPlacesSample();

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        string productName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch/products/{createdProduct.Id}";

        // Remove fulfillment from product.
        sample.PerformRemoveFulfillment(productName);

        // Get product.
        Product productWithUpdatedFulfillment = GetProductSample.GetRetailProduct(productName);

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(productWithUpdatedFulfillment.Name);
    }
}