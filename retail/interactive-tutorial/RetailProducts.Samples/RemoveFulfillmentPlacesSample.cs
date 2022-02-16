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

// [START remove_fulfillment_places]

using Google.Cloud.Retail.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading;

/// <summary>
/// The remove fulfillment places sample class.
/// </summary>
public class RemoveFulfillmentPlacesSample
{
    private const string ProductId = "remove_fulfillment_test_product_id";

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
        var removeFulfillmentRequest = new RemoveFulfillmentPlacesRequest
        {
            Product = productName,
            Type = "pickup-in-store",
            RemoveTime = Timestamp.FromDateTime(RequestTimeStamp),
            AllowMissing = true
        };

        string[] placeIds = { "store0" };

        removeFulfillmentRequest.PlaceIds.Add(placeIds);

        Console.WriteLine("Remove fulfillment places. request:");
        Console.WriteLine(removeFulfillmentRequest);
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
    /// <param name="projectId">The current project id.</param>
    /// <returns>Retail product with removed fulfillment places.</returns>
    public Product PerformRemoveFulfillment(string projectId)
    {
        string productName = $"projects/{projectId}/locations/global/catalogs/default_catalog/branches/default_branch/products/{ProductId}";

        CreateProductSample.CreateRetailProductWithFulfillment(ProductId, projectId);
        RemoveFulfillment(productName);
        var inventoryProduct = GetProductSample.GetRetailProduct(productName);
        DeleteProductSample.DeleteRetailProduct(productName);

        return inventoryProduct;
    }
}
// [END remove_fulfillment_places]

/// <summary>
/// The remove fulfillment places tutorial class.
/// </summary>
public static class RemoveFulfillmentPlacesTutorial
{
    [Runner.Attributes.Example]
    public static Product PerformRemoveFulfillment()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new RemoveFulfillmentPlacesSample();
        return sample.PerformRemoveFulfillment(projectId);
    }
}