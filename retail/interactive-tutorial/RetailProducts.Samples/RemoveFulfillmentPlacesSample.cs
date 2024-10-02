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
/// The remove fulfillment places sample class.
/// </summary>
public class RemoveFulfillmentPlaces
{
    /// <summary>
    /// Get the remove fulfillment places request.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    /// <returns>The remove fulfillment places request.</returns>
    private static RemoveFulfillmentPlacesRequest GetRemoveFulfillmentRequest(string productName)
    {
        var removeFulfillmentRequest = new RemoveFulfillmentPlacesRequest
        {
            Product = productName,
            Type = "pickup-in-store",
            AllowMissing = true,
            PlaceIds = { "store0" }
        };

        // To send an out-of-order request assign the invalid RemoveTime here:
        // removeFulfillmentRequest.RemoveTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-1));

        Console.WriteLine("Remove fulfillment places request:");
        Console.WriteLine($"Product Name: {removeFulfillmentRequest.Product}");
        Console.WriteLine($"Type: {removeFulfillmentRequest.Type}");
        Console.WriteLine($"Fulfillment Places: {removeFulfillmentRequest.PlaceIds}");
        Console.WriteLine();

        return removeFulfillmentRequest;
    }

    /// <summary>
    /// Call the Retail API to remove fulfillment places.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    public static void RemoveFulfillment(string productName)
    {
        var removeFulfillmentRequest = GetRemoveFulfillmentRequest(productName);
        var client = ProductServiceClient.Create();
        var operation = client.RemoveFulfillmentPlaces(removeFulfillmentRequest);

        Console.WriteLine("Remove fulfillment places. Please, wait.");
        Console.WriteLine();

        operation.PollUntilCompleted();
    }
}

/// <summary>
/// The remove fulfillment places tutorial class.
/// </summary>
public static class RemoveFulfillmentPlacesTutorial
{
    [Runner.Attributes.Example]
    public static void PerformAddRemoveFulfillment()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Remove fulfillment from product.
        RemoveFulfillmentPlaces.RemoveFulfillment(createdProduct.Name);

        // Get product.
        GetProductSample.GetRetailProduct(createdProduct.Name);

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
    }
}
