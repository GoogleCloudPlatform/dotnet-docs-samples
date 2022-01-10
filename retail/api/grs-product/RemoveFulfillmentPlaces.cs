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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;

namespace grs_product
{
    public static class RemoveFulfillmentPlaces
    {
        private const string Endpoint = "retail.googleapis.com";
        private const string ProductId = "remove_fulfillment_test_product_id";

        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string ProductName = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/branches/default_branch/products/{ProductId}";

        // The request timestamp
        private static readonly DateTime RequestTimeStamp = DateTime.Now.ToUniversalTime();

        // The outdated request timestamp
        // request_time = datetime.datetime.now() - datetime.timedelta(days=1)

        private static ProductServiceClient GetProductServiceClient()
        {
            var productServiceClientBuilder = new ProductServiceClientBuilder
            {
                Endpoint = Endpoint
            };

            var productServiceClient = productServiceClientBuilder.Build();
            return productServiceClient;
        }

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

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var removeFulfillmentRequestJson = JsonConvert.SerializeObject(removeFulfillmentRequest, jsonSerializeSettings);

            Console.WriteLine("\nRemove fulfillment places. request: \n" + removeFulfillmentRequestJson);
            return removeFulfillmentRequest;
        }

        private static void RemoveFulfillment(string productName)
        {
            var removeFulfillmentRequest = GetRemoveFulfillmentRequest(productName);
            GetProductServiceClient().RemoveFulfillmentPlaces(removeFulfillmentRequest);

            //This is a long running operation and its result is not immediately present with get operations,
            // thus we simulate wait with sleep method.
            Console.WriteLine("\nRemove fulfillment places. Wait 2 minutes:");
            Thread.Sleep(120000);
        }

        [Attributes.Example]
        public static Product PerformAddRemoveFulfillment()
        {
            CreateProduct.CreateRetailProductWithFulfillment(ProductId);
            RemoveFulfillment(ProductName);
            var inventoryProduct = GetProduct.GetRetailProduct(ProductName);
            // DeleteProduct.DeleteRetailProduct(ProductName);

            return inventoryProduct;
        }
    }
}
// [END retail_remove_fulfillment_places]