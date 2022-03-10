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

// [START retail_add_fulfillment_places]

using Google.Cloud.Retail.V2;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

/// <summary>
/// The add fulfillment places sample class.
/// </summary>
public class AddFulfillmentPlacesSample
{
    // The request timestamp
    private static readonly DateTime RequestTimeStamp = DateTime.Now.ToUniversalTime();

    // The outdated request timestamp:
    // RequestTimeStamp = DateTime.Now.ToUniversalTime().AddDays(-1);

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
            AddTime = Timestamp.FromDateTime(RequestTimeStamp.AddMinutes(-1)),
            AllowMissing = true
        };

        string[] placeIds = { "store2", "store3", "store4" };

        addFulfillmentRequest.PlaceIds.AddRange(placeIds);

        Console.WriteLine("Add fulfillment places request:");
        Console.WriteLine($"Product Name: {addFulfillmentRequest.Product}");
        Console.WriteLine($"Type: {addFulfillmentRequest.Type}");
        Console.WriteLine($"Add Time: {addFulfillmentRequest.AddTime}");
        Console.WriteLine();

        return addFulfillmentRequest;
    }

    /// <summary>
    /// Call the Retail API to add fulfillment places.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    public static void AddFulfillment(string productName)
    {
        AddFulfillmentPlacesRequest addFulfillmentRequest = GetAddFulfillmentRequest(productName);
        ProductServiceClient client = ProductServiceClient.Create();

        // Make the request.
        Operation<AddFulfillmentPlacesResponse, AddFulfillmentPlacesMetadata> response = client.AddFulfillmentPlaces(addFulfillmentRequest);

        Console.WriteLine("The operation was started:");
        Console.WriteLine(response.Name);
        Console.WriteLine();

        Console.WriteLine("Please wait till opeartion is done");
        Console.WriteLine();

        // Poll until the returned long-running operation is complete.
        Operation<AddFulfillmentPlacesResponse, AddFulfillmentPlacesMetadata> addFulfillmentResult = response.PollUntilCompleted();

        Console.WriteLine("Add fulfillment places operation is done");
        Console.WriteLine();

        Console.WriteLine("Operation result:");
        Console.WriteLine(addFulfillmentResult.Result);
        Console.WriteLine();
    }
}
// [END retail_add_fulfillment_places]

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
        Product inventoryProduct = GetProductSample.GetRetailProduct(createdProduct.Name);

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(inventoryProduct.Name);
    }
}