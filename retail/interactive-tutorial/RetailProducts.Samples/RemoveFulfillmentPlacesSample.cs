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

// [START retail_remove_fulfillment_places]

using Google.Cloud.Retail.V2;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;

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
        // The request timestamp
        DateTime requestTimeStamp = DateTime.Now.ToUniversalTime();

        // The outdated request timestamp
        // requestTimeStamp = DateTime.Now.ToUniversalTime().AddDays(-1);

        var removeFulfillmentRequest = new RemoveFulfillmentPlacesRequest
        {
            Product = productName,
            Type = "pickup-in-store",
            RemoveTime = Timestamp.FromDateTime(requestTimeStamp),
            AllowMissing = true
        };

        string[] placeIds = { "store0" };

        removeFulfillmentRequest.PlaceIds.Add(placeIds);

        var jsonSerializeSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        var removeFulfillmentRequestJson = JsonConvert.SerializeObject(removeFulfillmentRequest, jsonSerializeSettings);

        Console.WriteLine("\nRemove fulfillment places request: \n" + removeFulfillmentRequestJson);
        return removeFulfillmentRequest;
    }

    /// <summary>
    /// Call the Retail API to remove fulfillment places.
    /// </summary>
    /// <param name="productName">The actual name of the retail product.</param>
    public static void RemoveFulfillment(string productName)
    {
        var removeFulfillmentRequest = GetRemoveFulfillmentRequest(productName);
        GetProductServiceClient().RemoveFulfillmentPlaces(removeFulfillmentRequest);

        //This is a long running operation and its result is not immediately present with get operations,
        // thus we simulate wait with sleep method.
        Console.WriteLine("\nRemove fulfillment places. Wait 2 minutes:");
        Thread.Sleep(120000);
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
// [END retail_remove_fulfillment_places]

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