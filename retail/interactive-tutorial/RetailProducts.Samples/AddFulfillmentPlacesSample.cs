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
/// The add fulfillment places sample class.
/// </summary>
public class AddFulfillmentPlacesSample
{
    /// <summary>
    /// Get the add fulfillment palces request.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    /// <returns>Add fulfillment places request.</returns>
    private static AddFulfillmentPlacesRequest GetAddFulfillmentRequest(string productName)
    {
        AddFulfillmentPlacesRequest addFulfillmentRequest = new AddFulfillmentPlacesRequest
        {
            Product = productName,
            Type = "pickup-in-store",
            AllowMissing = true,
            PlaceIds = { "store2", "store3", "store4" }
        };

        // To send an out-of-order request assign the invalid AddTime here:
        // addFulfillmentRequest.AddTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-1));

        Console.WriteLine("Add fulfillment places request:");
        Console.WriteLine($"Product Name: {addFulfillmentRequest.Product}");
        Console.WriteLine($"Type: {addFulfillmentRequest.Type}");
        Console.WriteLine($"Fulfillment Places: {addFulfillmentRequest.PlaceIds}");
        Console.WriteLine();

        return addFulfillmentRequest;
    }

    /// <summary>
    /// Call the Retail API to add fulfillment places.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    public static void AddFulfillment(string productName)
    {
        var addFulfillmentRequest = GetAddFulfillmentRequest(productName);
        var client = ProductServiceClient.Create();
        var operation = client.AddFulfillmentPlaces(addFulfillmentRequest);

        Console.WriteLine("Add fulfillment places. Please, wait.");
        Console.WriteLine();

        operation.PollUntilCompleted();
    }
}

/// <summary>
/// The add fulfillment places tutorial class.
/// </summary>
public static class AddFulfillmentPlacesTutorial
{
    [Runner.Attributes.Example]
    public static void PerformAddFulfillment()
    {
        string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        // Add fulfillment info to created product.
        AddFulfillmentPlacesSample.AddFulfillment(createdProduct.Name);

        // Get product.
        GetProductSample.GetRetailProduct(createdProduct.Name);

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(createdProduct.Name);
    }
}
