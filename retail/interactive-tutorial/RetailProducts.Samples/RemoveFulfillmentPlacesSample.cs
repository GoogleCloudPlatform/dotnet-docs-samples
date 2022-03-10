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
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System;

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

        Console.WriteLine("Remove fulfillment places request:");
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
    public static void RemoveFulfillment(string productName)
    {
        RemoveFulfillmentPlacesRequest removeFulfillmentRequest = GetRemoveFulfillmentRequest(productName);

        ProductServiceClient client = ProductServiceClient.Create();

        // Make the request.
        Operation<RemoveFulfillmentPlacesResponse, RemoveFulfillmentPlacesMetadata> response = client.RemoveFulfillmentPlaces(removeFulfillmentRequest);

        Console.WriteLine("The operation was started:");
        Console.WriteLine(response.Name);
        Console.WriteLine();

        Console.WriteLine("Please wait till opeartion is done");
        Console.WriteLine();

        // Poll until the returned long-running operation is complete.
        Operation<RemoveFulfillmentPlacesResponse, RemoveFulfillmentPlacesMetadata> removeFulfillmentResult = response.PollUntilCompleted();

        Console.WriteLine("Remove fulfillment places operation is done");
        Console.WriteLine();

        Console.WriteLine("Operation result:");
        Console.WriteLine(removeFulfillmentResult.Result);
        Console.WriteLine();
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

        // Create product.
        Product createdProduct = CreateProductSample.CreateRetailProductWithFulfillment(projectId);

        string productName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch/products/{createdProduct.Id}";

        // Remove fulfillment from product.
        RemoveFulfillmentPlacesSample.RemoveFulfillment(productName);

        // Get product.
        Product productWithUpdatedFulfillment = GetProductSample.GetRetailProduct(productName);

        // Delete product.
        DeleteProductSample.DeleteRetailProduct(productWithUpdatedFulfillment.Name);
    }
}